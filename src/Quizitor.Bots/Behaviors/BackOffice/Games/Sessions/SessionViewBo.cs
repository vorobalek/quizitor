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

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<ISessionViewBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<ISessionViewBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class SessionViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<ISessionViewBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>session</b>.{sessionId}.{gamePageNumber}.{sessionPageNumber}
    /// </summary>
    public const string Button = "session";

    public override string[] Permissions => [UserPermission.BackOfficeSessionView];

    protected virtual string ButtonInternal => Button;

    public string CallbackQueryDataPrefixValue => $"{ButtonInternal}.";

    public virtual Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<ISessionViewBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } sessionIdString,
                {
                } gamePageNumberString,
                {
                } sessionPageNumberString
            ] &&
            int.TryParse(sessionIdString, out var sessionId) &&
            int.TryParse(gamePageNumberString, out var gamePageNumber) &&
            int.TryParse(sessionPageNumberString, out var sessionPageNumber) &&
            await dbContextProvider.Sessions.GetByIdOrDefaultAsync(sessionId, cancellationToken) is { } session)
        {
            var game = await dbContextProvider
                .Games
                .GetByIdAsync(
                    session.GameId,
                    cancellationToken);

            var usersCount = await dbContextProvider
                .Users
                .CountBySessionIdAsync(
                    session.Id,
                    cancellationToken);

            var submissionsCount = await dbContextProvider
                .Submissions
                .CountBySessionIdAsync(
                    session.Id,
                    cancellationToken);

            return ISessionViewBackOfficeContext.Create(
                session,
                game,
                usersCount,
                submissionsCount,
                gamePageNumber,
                sessionPageNumber,
                backOfficeContext);
        }

        return null;
    }

    private static Task ResponseAsync<TContext>(
        IBehaviorTraitContext<TContext>? context,
        CancellationToken cancellationToken)
        where TContext : ISessionViewBackOfficeContext
    {
        if (context is null) return Task.CompletedTask;

        var text = string.Format(
            TR.L + "_BACKOFFICE_SESSION_VIEW_TXT",
            context.Base.Game.Title.EscapeHtml(),
            context.Base.Session.Name.EscapeHtml(),
            context.Base.UsersCount,
            context.Base.SubmissionsCount);
        var keyboard = Keyboards.Session(
            context.Base.Session.GameId,
            context.Base.Session.Id,
            context.Base.GamePageNumber,
            context.Base.SessionPageNumber);

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