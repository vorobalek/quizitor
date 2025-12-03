using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin;

using MessageBehavior = IMessageTextBotCommandEqualsBehaviorTrait<IGameAdminContext>;
using MessageContext = IMessageTextBotCommandEqualsContext<IGameAdminContext>;
using CallbackQueryBehavior = ICallbackQueryDataEqualsBehaviorTrait<IGameAdminContext>;
using CallbackQueryContext = ICallbackQueryDataEqualsContext<IGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MainPageGa(
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior(
        dbContextProvider),
    MessageBehavior,
    CallbackQueryBehavior
{
    public const string Button = "start";
    protected override string[] GameAdminPermissions => [UserPermission.GameAdminMain];

    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        CallbackQueryContext context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    public string BotCommandValue => Button;

    public Task PerformMessageTextBotCommandEqualsAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    public static Task ResponseAsync<TContext>(
        TContext? context,
        CancellationToken cancellationToken)
        where TContext : IBehaviorTraitContext<IGameAdminContext>
    {
        if (context is null) return Task.CompletedTask;

        var text = string.Format(
            TR.L + "_GAME_ADMIN_MAIN_TXT",
            context.Base.Identity.User.FirstName.Html,
            context.Base.Identity.User.LastName?.Html,
            string.Format(
                TR.L + "_GAME_ADMIN_MAIN_GAME_INFO_TXT",
                context.Base.Game.Title.Html,
                context.Base.Session.Name.Html));
        var keyboard = Keyboards.MainInline;
        return context switch
        {
            ICallbackQueryDataEqualsContext callbackQueryDataEqualsContext =>
                context
                    .Base
                    .Client
                    .EditMessageText(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        callbackQueryDataEqualsContext.MessageId,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            IMessageTextBotCommandEqualsContext or
                IQrCodeDataPrefixContext =>
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