using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<ISessionListBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<ISessionListBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class SessionListBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<ISessionListBackOfficeContext>,
    Behavior
{
    public const int PageSize = 10;

    /// <summary>
    ///     <b>sessions</b>.{gameId}.{gamePageNumber}.{sessionPageNumber}
    /// </summary>
    public const string Button = "sessions";

    public override string[] Permissions => [UserPermission.BackOfficeSessionList];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<ISessionListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
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
            var sessions = await dbContextProvider
                .Sessions
                .GetPaginatedByGameIdAsync(
                    gameId,
                    sessionPageNumber,
                    PageSize,
                    cancellationToken);

            var sessionPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await dbContextProvider
                            .Sessions
                            .CountByGameIdAsync(
                                gameId,
                                cancellationToken)) / PageSize));

            return ISessionListBackOfficeContext.Create(
                game,
                sessions,
                gamePageNumber,
                sessionPageNumber,
                sessionPageCount,
                backOfficeContext);
        }

        return null;
    }

    public static Task ResponseAsync<TContext>(
        TContext? context,
        CancellationToken cancellationToken)
        where TContext : IBehaviorTraitContext<ISessionListBackOfficeContext>
    {
        if (context is null) return Task.CompletedTask;

        var text = string
            .Format(
                TR.L + "_BACKOFFICE_SESSIONS_TXT",
                context.Base.Game.Title.EscapeHtml());
        var keyboard = Keyboards.SessionList(
            context.Base.Sessions,
            context.Base.Game.Id,
            context.Base.GamePageNumber,
            context.Base.SessionPageNumber,
            context.Base.SessionPageCount);

        return context switch
        {
            ICallbackQueryDataPrefixContext callbackQueryDataPrefixContext =>
                context
                    .Base
                    .Client
                    .EditMessageText(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        callbackQueryDataPrefixContext.MessageId,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            IMessageTextContext =>
                context
                    .Base
                    .Client
                    .SendMessage(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            _ => Task.CompletedTask
        };
    }
}