using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IBotBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IBotBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class BotViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IBotBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>bot</b>.{botId}.{botPageNumber}
    /// </summary>
    public const string Button = "bot";

    public override string[] Permissions => [UserPermission.BackOfficeBotView];

    protected virtual string ButtonInternal => Button;

    public string CallbackQueryDataPrefixValue => $"{ButtonInternal}.";

    public virtual Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<IBotBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } botIdString,
                {
                } botPageNumberString
            ] &&
            int.TryParse(botIdString, out var botId) &&
            int.TryParse(botPageNumberString, out var botPageNumber) &&
            await dbContextProvider.Bots.GetByIdOrDefaultAsync(botId, cancellationToken) is { } bot)
        {
            var gameServerUsersCount = await dbContextProvider
                .Users
                .CountByGameServerIdAsync(
                    bot.Id,
                    cancellationToken);
            var gameAdminUsersCount = await dbContextProvider
                .Users
                .CountByGameAdminIdAsync(
                    bot.Id,
                    cancellationToken);

            return IBotBackOfficeContext.Create(
                bot,
                gameServerUsersCount,
                gameAdminUsersCount,
                botPageNumber,
                backOfficeContext);
        }

        return null;
    }

    protected static Task ResponseAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                string.Format(
                    TR.L + "_BACKOFFICE_BOT_VIEW_TXT",
                    context.Base.Bot.Name.EscapeHtml(),
                    context.Base.Bot.IsActive
                        ? TR.L + "_BACKOFFICE_BOT_ACTIVE_TXT"
                        : TR.L + "_BACKOFFICE_BOT_INACTIVE_TXT",
                    context.Base.Bot.Type,
                    context.Base.Bot.Username?.EscapeHtml(),
                    context.Base.Bot.DropPendingUpdates
                        ? TR.L + "_SHARED_YES_TXT"
                        : TR.L + "_SHARED_NO_TXT",
                    context.Base.GameServerUsersCount,
                    context.Base.GameAdminUsersCount),
                ParseMode.Html,
                replyMarkup: Keyboards.BotView(
                    context.Base.Bot,
                    context.Base.BotPageNumber), cancellationToken: cancellationToken);
    }
}