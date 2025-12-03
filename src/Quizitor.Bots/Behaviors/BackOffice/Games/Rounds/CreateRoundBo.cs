using System.Text.Json;
using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Infrastructure;
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

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds;

using CallbackQueryBehavior = ICallbackQueryDataPrefixBehaviorTrait<ICreateRoundBackOfficeContext>;
using CallbackQueryContext = ICallbackQueryDataPrefixContext<ICreateRoundBackOfficeContext>;
using MessageBehavior = IMessageTextBehaviorTrait<ICreateRoundBackOfficeContext>;
using MessageContext = IMessageTextContext<ICreateRoundBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CreateRoundBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<ICreateRoundBackOfficeContext>,
    CallbackQueryBehavior,
    MessageBehavior
{
    /// <summary>
    ///     <b>createround</b>.{gameId}.{gamePageNumber}.{roundPageNumber}
    /// </summary>
    public const string Button = "createround";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeRoundCreate,
        UserPermission.BackOfficeRoundList
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
                    UserPromptType.BackOfficeNewRoundTitle,
                    JsonSerializer.Serialize(
                        new PromptSubject(
                            newPrompt.Game.Id,
                            newPrompt.GamePageNumber,
                            newPrompt.RoundPageNumber)),
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_MESSAGE_{UserPromptType.BackOfficeNewRoundTitle}_TXT",
                            ParseMode.Html,
                            replyMarkup: Keyboards.CancelPrompt,
                            cancellationToken: cancellationToken));
        }
    }

    public override bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return !IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update) &&
               base.ShouldPerformMessageText(baseContext) &&
               baseContext.Identity.Prompt?.Type is UserPromptType.BackOfficeNewRoundTitle;
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
                            TR.L + "_BACKOFFICE_ROUND_NOT_CREATED_ERROR_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return;
        }

        if (context.Base.NewRound is { } newRound)
        {
            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);
            await dbContextProvider.Users.UpdateAsync(
                context.Base.Identity.User,
                cancellationToken);

            var round = new Round
            {
                Title = newRound.RoundTitle,
                Game = newRound.Game,
                Number = newRound.RoundNumber
            };
            await dbContextProvider
                .Rounds
                .AddAsync(
                    round,
                    cancellationToken);

            var roundListContext = IMessageTextContext.Create(
                IRoundListBackOfficeContext.Create(
                    newRound.Game,
                    [round, .. newRound.Rounds],
                    newRound.RoundsCount,
                    newRound.GamePageNumber,
                    0,
                    newRound.RoundPageCount,
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
                                    TR.L + "_BACKOFFICE_ROUND_CREATED_TXT",
                                    round.Title.Html,
                                    newRound.Game.Title.Html),
                            ParseMode.Html,
                            cancellationToken: cancellationToken);
                    await RoundListBo.ResponseAsync(roundListContext, cancellationToken);
                });
        }
    }

    protected override async Task<ICreateRoundBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        if (ICallbackQueryDataPrefixContext.IsValidUpdate(backOfficeContext.UpdateContext.Update, CallbackQueryDataPrefixValue))
        {
            var callbackQueryContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);

            if (callbackQueryContext?.Base.Identity.Prompt?.Type is { } promptType)
            {
                return ICreateRoundBackOfficeContext.Create(
                    ICreateRoundBackOfficeContext.IWrongPrompt.Create(promptType),
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
                    } roundPageNumberString
                ] &&
                int.TryParse(gameIdString, out var gameId) &&
                int.TryParse(gamePageNumberString, out var gamePageNumber) &&
                int.TryParse(roundPageNumberString, out var roundPageNumber) &&
                await dbContextProvider.Games.GetByIdOrDefaultAsync(gameId, cancellationToken) is { } game)
            {
                return ICreateRoundBackOfficeContext.Create(
                    null,
                    ICreateRoundBackOfficeContext.INewPrompt.Create(
                        game,
                        gamePageNumber,
                        roundPageNumber),
                    null,
                    null,
                    callbackQueryContext.Base);
            }
        }

        if (IMessageTextContext.IsValidUpdate(backOfficeContext.UpdateContext.Update))
        {
            if (backOfficeContext.Identity.Prompt?.Type is not UserPromptType.BackOfficeNewRoundTitle) return null;
            var messageTextContext = IMessageTextContext.Create(backOfficeContext);

            if (messageTextContext?.Base.Identity.Prompt?.Subject is not { } promptSubjectString ||
                JsonSerializerHelper.TryDeserialize<PromptSubject>(promptSubjectString) is not
                {
                    GameId: { } gameId,
                    GamePageNumber: { } gamePageNumber,
                    RoundPageNumber: { } roundPageNumber
                } ||
                await dbContextProvider.Games.GetByIdOrDefaultAsync(gameId, cancellationToken) is not { } game)
            {
                return ICreateRoundBackOfficeContext.Create(
                    null,
                    null,
                    ICreateRoundBackOfficeContext.IDataError.Create(),
                    null,
                    backOfficeContext);
            }

            var rounds = await dbContextProvider
                .Rounds
                .GetPaginatedAfterCreationByGameIdAsync(
                    game.Id,
                    RoundListBo.PageSize,
                    cancellationToken);

            var roundsCount = await dbContextProvider
                .Rounds
                .CountByGameIdAsync(
                    game.Id,
                    cancellationToken) + 1;

            var roundPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(roundsCount) / RoundListBo.PageSize));

            var roundNumber = await dbContextProvider
                .Rounds
                .GetNextNumberByGameIdAsync(
                    game.Id,
                    cancellationToken);

            return ICreateRoundBackOfficeContext.Create(
                null,
                null,
                null,
                ICreateRoundBackOfficeContext.INewRound.Create(
                    messageTextContext.MessageText.Truncate(256),
                    game,
                    rounds,
                    roundsCount,
                    roundNumber,
                    gamePageNumber,
                    roundPageNumber,
                    roundPageCount),
                backOfficeContext);
        }

        return null;
    }

    private record PromptSubject(
        int? GameId,
        int? GamePageNumber,
        int? RoundPageNumber);
}