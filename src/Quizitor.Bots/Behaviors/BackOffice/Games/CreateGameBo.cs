using System.Text.Json;
using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;
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
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games;

using CallbackQueryBehavior = ICallbackQueryDataPrefixBehaviorTrait<ICreateGameBackOfficeContext>;
using CallbackQueryContext = ICallbackQueryDataPrefixContext<ICreateGameBackOfficeContext>;
using MessageBehavior = IMessageTextBehaviorTrait<ICreateGameBackOfficeContext>;
using MessageContext = IMessageTextContext<ICreateGameBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CreateGameBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<ICreateGameBackOfficeContext>,
    CallbackQueryBehavior,
    MessageBehavior
{
    /// <summary>
    ///     <b>creategame</b>.{gamePageNumber}
    /// </summary>
    public const string Button = "creategame";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeGameCreate,
        UserPermission.BackOfficeGameList
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
                    UserPromptType.BackOfficeNewGameTitle,
                    JsonSerializer.Serialize(
                        new PromptSubject(newPrompt.GamePageNumber)),
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_MESSAGE_{UserPromptType.BackOfficeNewGameTitle}_TXT",
                            ParseMode.Html,
                            replyMarkup: Keyboards.CancelPrompt,
                            cancellationToken: cancellationToken));
        }
    }

    public override bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return !IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update) &&
               base.ShouldPerformMessageText(baseContext) &&
               baseContext.Identity.Prompt?.Type is UserPromptType.BackOfficeNewGameTitle;
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
                            TR.L + "_BACKOFFICE_GAME_NOT_CREATED_ERROR_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return;
        }

        if (context.Base.NewGame is { } newGame)
        {
            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);

            var game = new Game
            {
                Title = newGame.GameTitle
            };
            await dbContextProvider
                .Games
                .AddAsync(
                    game,
                    cancellationToken);

            var gameListContext = IMessageTextContext.Create(
                IGameListBackOfficeContext.Create(
                    [game, .. newGame.Games],
                    newGame.GamePageNumber,
                    newGame.GamePageCount,
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
                                    TR.L + "_BACKOFFICE_GAME_CREATED_TXT",
                                    game.Title.EscapeHtml()),
                            ParseMode.Html,
                            cancellationToken: cancellationToken);

                    await GameListBo.ResponseAsync(gameListContext, cancellationToken);
                });
        }
    }

    protected override async Task<ICreateGameBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        if (ICallbackQueryDataPrefixContext.IsValidUpdate(backOfficeContext.UpdateContext.Update, CallbackQueryDataPrefixValue))
        {
            var callbackQueryContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);

            if (callbackQueryContext?.Base.Identity.Prompt?.Type is { } promptType)
            {
                return ICreateGameBackOfficeContext.Create(
                    ICreateGameBackOfficeContext.IWrongPrompt.Create(promptType),
                    null,
                    null,
                    null,
                    backOfficeContext);
            }

            if (callbackQueryContext?.CallbackQueryDataPostfix.Split('.') is
                [
                    {
                    } gamePageNumberString
                ] &&
                int.TryParse(gamePageNumberString, out var gamePageNumber))
            {
                return ICreateGameBackOfficeContext.Create(
                    null,
                    ICreateGameBackOfficeContext.INewPrompt.Create(gamePageNumber),
                    null,
                    null,
                    callbackQueryContext.Base);
            }
        }

        if (IMessageTextContext.IsValidUpdate(backOfficeContext.UpdateContext.Update))
        {
            if (backOfficeContext.Identity.Prompt?.Type is not UserPromptType.BackOfficeNewGameTitle) return null;
            var messageTextContext = IMessageTextContext.Create(backOfficeContext);

            if (messageTextContext?.Base.Identity.Prompt?.Subject is not { } promptSubjectString ||
                JsonSerializerHelper.TryDeserialize<PromptSubject>(promptSubjectString) is not
                {
                    GamePageNumber: { } gamePageNumber
                })
            {
                return ICreateGameBackOfficeContext.Create(
                    null,
                    null,
                    ICreateGameBackOfficeContext.IDataError.Create(),
                    null,
                    backOfficeContext);
            }

            var games = await dbContextProvider
                .Games
                .GetPaginatedAfterCreationAsync(
                    GameListBo.PageSize,
                    cancellationToken);

            var gamePageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await dbContextProvider
                            .Games
                            .CountAsync(cancellationToken) + 1) / GameListBo.PageSize));

            return ICreateGameBackOfficeContext.Create(
                null,
                null,
                null,
                ICreateGameBackOfficeContext.INewGame.Create(
                    messageTextContext.MessageText.Truncate(256),
                    games,
                    gamePageNumber,
                    gamePageCount),
                backOfficeContext);
        }

        return null;
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record PromptSubject(int? GamePageNumber);
}