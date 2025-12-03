using System.Text.Json;
using LPlus;
using Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.UI.Shared;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using CallbackQueryBehavior = ICallbackQueryDataPrefixBehaviorTrait<ICreateTeamGameServerContext>;
using CallbackQueryContext = ICallbackQueryDataPrefixContext<ICreateTeamGameServerContext>;
using MessageBehavior = IMessageTextBehaviorTrait<ICreateTeamGameServerContext>;
using MessageContext = IMessageTextContext<ICreateTeamGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CreateTeamGs(
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<ICreateTeamGameServerContext>(dbContextProvider),
    CallbackQueryBehavior,
    MessageBehavior
{
    /// <summary>
    ///     <b>createteam</b>.{teamPageNumber}
    /// </summary>
    public const string Button = "createteam";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        CallbackQueryContext context,
        CancellationToken cancellationToken)
    {
        if (context.Base.WrongPrompt is { } wrongPrompt)
        {
            await FallbackWrongUserPromptAsync(
                context,
                wrongPrompt.PromptType,
                cancellationToken);
            return;
        }

        if (context.Base.NewPrompt is { } newPrompt)
        {
            await _dbContextProvider
                .Users
                .SetPromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    UserPromptType.GameServerNewTeamName,
                    JsonSerializer.Serialize(
                        new PromptSubject(newPrompt.TeamPageNumber)),
                    cancellationToken);
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_MESSAGE_{UserPromptType.GameServerNewTeamName}_TXT",
                            ParseMode.Html,
                            replyMarkup: Keyboards.CancelPrompt,
                            cancellationToken: cancellationToken));
        }
    }

    public override bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return !IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update) &&
               baseContext.Identity.Prompt?.Type is UserPromptType.GameServerNewTeamName &&
               base.ShouldPerformMessageText(baseContext);
    }

    public async Task PerformMessageTextAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        if (context.Base.DataError is { } dataError)
        {
            await _dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            dataError.Error != null
                                ? string.Format(
                                    TR.L + "_GAME_SERVER_TEAM_NOT_CREATED_ERROR_TXT",
                                    dataError.Error)
                                : TR.L + "_GAME_SERVER_TEAM_NOT_CREATED_UNKNOWN_ERROR_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return;
        }

        if (context.Base.NewTeam is { } newTeam)
        {
            await _dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);

            var team = new Team
            {
                Name = newTeam.TeamName,
                OwnerId = context.Base.Identity.User.Id
            };
            await _dbContextProvider
                .Teams
                .AddAsync(
                    team,
                    cancellationToken);

            var teamListContext = IMessageTextContext.Create(
                ITeamListGameServerContext.Create(
                    [team, .. newTeam.Teams],
                    newTeam.TeamPageNumber,
                    newTeam.TeamPageCount,
                    context.Base));

            _dbContextProvider
                .AddPostCommitTask(async () =>
                {
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            string
                                .Format(
                                    TR.L + "_GAME_SERVER_TEAM_CREATED_TXT",
                                    team.Name.Html),
                            ParseMode.Html,
                            cancellationToken: cancellationToken);

                    await TeamListGs.ResponseAsync(teamListContext, cancellationToken);
                });
        }
    }

    protected override async Task<ICreateTeamGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        if (ICallbackQueryDataPrefixContext.IsValidUpdate(gameServerContext.UpdateContext.Update, CallbackQueryDataPrefixValue))
        {
            var callbackQueryContext = ICallbackQueryDataPrefixContext.Create(gameServerContext, CallbackQueryDataPrefixValue);

            if (callbackQueryContext?.Base.Identity.Prompt?.Type is { } promptType)
            {
                return ICreateTeamGameServerContext.Create(
                    ICreateTeamGameServerContext.IWrongPrompt.Create(promptType),
                    null,
                    null,
                    null,
                    gameServerContext);
            }

            if (callbackQueryContext?.CallbackQueryDataPostfix.Split('.') is
                [
                    {
                    } gamePageNumberString
                ] &&
                int.TryParse(gamePageNumberString, out var gamePageNumber))
            {
                return ICreateTeamGameServerContext.Create(
                    null,
                    ICreateTeamGameServerContext.INewPrompt.Create(gamePageNumber),
                    null,
                    null,
                    callbackQueryContext.Base);
            }
        }

        if (IMessageTextContext.IsValidUpdate(gameServerContext.UpdateContext.Update))
        {
            if (gameServerContext.Identity.Prompt?.Type is not UserPromptType.GameServerNewTeamName) return null;
            var messageTextContext = IMessageTextContext.Create(gameServerContext);

            if (messageTextContext?.Base.Identity.Prompt?.Subject is not { } promptSubjectString ||
                JsonSerializerHelper.TryDeserialize<PromptSubject>(promptSubjectString) is not
                {
                    TeamPageNumber: { } gamePageNumber
                })
            {
                return ICreateTeamGameServerContext.Create(
                    null,
                    null,
                    ICreateTeamGameServerContext.IDataError.Create(null),
                    null,
                    gameServerContext);
            }

            var teamName = messageTextContext.MessageText;
            if (teamName.Length > 80)
            {
                return ICreateTeamGameServerContext.Create(
                    null,
                    null,
                    ICreateTeamGameServerContext.IDataError.Create(TR.L + "_GAME_SERVER_TEAM_NAME_TOO_LONG_TXT"),
                    null,
                    gameServerContext);
            }

            var teamWithTheSameName = await _dbContextProvider
                .Teams
                .GetByNameOrDefaultAsync(
                    teamName,
                    cancellationToken);

            if (teamWithTheSameName != null)
            {
                return ICreateTeamGameServerContext.Create(
                    null,
                    null,
                    ICreateTeamGameServerContext.IDataError.Create(
                        string.Format(
                            TR.L + "_GAME_SERVER_TEAM_NAME_OCCUPIED_TXT",
                            teamName.Html)),
                    null,
                    gameServerContext);
            }

            var teams = await _dbContextProvider
                .Teams
                .GetPaginatedByOwnerIdAfterCreationAsync(
                    gameServerContext.Identity.User.Id,
                    TeamListGs.PageSize,
                    cancellationToken);

            var teamPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await _dbContextProvider
                            .Teams
                            .CountByOwnerIdAsync(
                                gameServerContext.Identity.User.Id,
                                cancellationToken) + 1) / TeamListGs.PageSize));

            return ICreateTeamGameServerContext.Create(
                null,
                null,
                null,
                ICreateTeamGameServerContext.INewTeam.Create(
                    teamName,
                    teams,
                    gamePageNumber,
                    teamPageCount),
                gameServerContext);
        }

        return null;
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record PromptSubject(int? TeamPageNumber);
}