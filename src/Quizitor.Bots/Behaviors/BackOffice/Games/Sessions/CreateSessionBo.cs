using System.Text.Json;
using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions.Infrastructure;
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

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;

using CallbackQueryBehavior = ICallbackQueryDataPrefixBehaviorTrait<ICreateSessionBackOfficeContext>;
using CallbackQueryContext = ICallbackQueryDataPrefixContext<ICreateSessionBackOfficeContext>;
using MessageBehavior = IMessageTextBehaviorTrait<ICreateSessionBackOfficeContext>;
using MessageContext = IMessageTextContext<ICreateSessionBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CreateSessionBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<ICreateSessionBackOfficeContext>,
    CallbackQueryBehavior,
    MessageBehavior
{
    /// <summary>
    ///     <b>createsession</b>.{gameId}.{gamePageNumber}.{sessionPageNumber}
    /// </summary>
    public const string Button = "createsession";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeSessionCreate,
        UserPermission.BackOfficeSessionList
    ];

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
            await dbContextProvider
                .Users
                .SetPromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    UserPromptType.BackOfficeNewSessionName,
                    JsonSerializer.Serialize(
                        new PromptSubject(
                            newPrompt.Game.Id,
                            newPrompt.GamePageNumber,
                            newPrompt.SessionPageNumber)),
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_MESSAGE_{UserPromptType.BackOfficeNewSessionName}_TXT",
                            ParseMode.Html,
                            replyMarkup: Keyboards.CancelPrompt,
                            cancellationToken: cancellationToken));
        }
    }

    public override bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return !IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update) &&
               base.ShouldPerformMessageText(baseContext) &&
               baseContext.Identity.Prompt?.Type is UserPromptType.BackOfficeNewSessionName;
    }

    public async Task PerformMessageTextAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        if (context.Base.DataError is not null)
        {
            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + "_BACKOFFICE_SESSION_NOT_CREATED_ERROR_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return;
        }

        if (context.Base.NewSession is { } newSession)
        {
            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);

            var session = new Session
            {
                Name = newSession.SessionName,
                Game = newSession.Game
            };
            await dbContextProvider
                .Sessions
                .AddAsync(
                    session,
                    cancellationToken);

            var sessionListContext = IMessageTextContext.Create(
                ISessionListBackOfficeContext.Create(
                    newSession.Game,
                    [session, .. newSession.Sessions],
                    newSession.GamePageNumber,
                    0,
                    newSession.SessionPageCount,
                    context.Base));

            dbContextProvider
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
                                    TR.L + "_BACKOFFICE_SESSION_CREATED_TXT",
                                    session.Name.EscapeHtml(),
                                    newSession.Game.Title.EscapeHtml()),
                            ParseMode.Html,
                            cancellationToken: cancellationToken);
                    await SessionListBo.ResponseAsync(sessionListContext, cancellationToken);
                });
        }
    }

    protected override async Task<ICreateSessionBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        if (ICallbackQueryDataPrefixContext.IsValidUpdate(backOfficeContext.UpdateContext.Update, CallbackQueryDataPrefixValue))
        {
            var callbackQueryContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);

            if (callbackQueryContext?.Base.Identity.Prompt?.Type is { } promptType)
            {
                return ICreateSessionBackOfficeContext.Create(
                    ICreateSessionBackOfficeContext.IWrongPrompt.Create(promptType),
                    null,
                    null,
                    null,
                    backOfficeContext);
            }

            if (callbackQueryContext?.CallbackQueryDataPostfix.Split('.') is
                [
                    {
                    } gameIdString,
                    {
                    } gamePageNumberString,
                    {
                    } sessionPageNumberString
                ] &&
                int.TryParse(gameIdString, out var gameId) &&
                int.TryParse(gamePageNumberString, out var gamePageNumber) &&
                int.TryParse(sessionPageNumberString, out var sessionPageNumber) &&
                await dbContextProvider.Games.GetByIdOrDefaultAsync(gameId, cancellationToken) is { } game)
            {
                return ICreateSessionBackOfficeContext.Create(
                    null,
                    ICreateSessionBackOfficeContext.INewPrompt.Create(
                        game,
                        gamePageNumber,
                        sessionPageNumber),
                    null,
                    null,
                    callbackQueryContext.Base);
            }
        }

        if (IMessageTextContext.IsValidUpdate(backOfficeContext.UpdateContext.Update))
        {
            if (backOfficeContext.Identity.Prompt?.Type is not UserPromptType.BackOfficeNewSessionName) return null;
            var messageTextContext = IMessageTextContext.Create(backOfficeContext);

            if (messageTextContext?.Base.Identity.Prompt?.Subject is not { } promptSubjectString ||
                JsonSerializerHelper.TryDeserialize<PromptSubject>(promptSubjectString) is not
                {
                    GameId: { } gameId,
                    GamePageNumber: { } gamePageNumber,
                    SessionPageNumber: { } sessionPageNumber
                } ||
                await dbContextProvider.Games.GetByIdOrDefaultAsync(gameId, cancellationToken) is not { } game)
            {
                return ICreateSessionBackOfficeContext.Create(
                    null,
                    null,
                    ICreateSessionBackOfficeContext.IDataError.Create(),
                    null,
                    backOfficeContext);
            }

            var sessions = await dbContextProvider
                .Sessions
                .GetPaginatedAfterCreationByGameIdAsync(
                    game.Id,
                    SessionListBo.PageSize,
                    cancellationToken);

            var sessionPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await dbContextProvider
                            .Sessions
                            .CountByGameIdAsync(
                                game.Id,
                                cancellationToken) + 1) / SessionListBo.PageSize));

            return ICreateSessionBackOfficeContext.Create(
                null,
                null,
                null,
                ICreateSessionBackOfficeContext.INewSession.Create(
                    messageTextContext.MessageText.Truncate(256),
                    game,
                    sessions,
                    gamePageNumber,
                    sessionPageNumber,
                    sessionPageCount),
                backOfficeContext);
        }

        return null;
    }

    private record PromptSubject(
        int? GameId,
        int? GamePageNumber,
        int? SessionPageNumber);
}