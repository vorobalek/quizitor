using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice;

using MessageBehavior = IMessageTextBotCommandEqualsBehaviorTrait<IBackOfficeContext>;
using MessageContext = IMessageTextBotCommandEqualsContext<IBackOfficeContext>;
using CallbackQueryBehavior = ICallbackQueryDataEqualsBehaviorTrait<IBackOfficeContext>;
using CallbackQueryContext = ICallbackQueryDataEqualsContext<IBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MainPageBo :
    BackOfficeBehavior,
    MessageBehavior,
    CallbackQueryBehavior
{
    /// <summary>
    ///     <b>start</b><br />
    ///     /<b>start</b>
    /// </summary>
    public const string Button = "start";

    public override string[] Permissions => [UserPermission.BackOfficeMain];

    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        CallbackQueryContext context,
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
                    TR.L + "_BACKOFFICE_MAIN_TXT",
                    context.Base.TelegramUser.FirstName.EscapeHtml(),
                    context.Base.TelegramUser.LastName?.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.MainPage, cancellationToken: cancellationToken);
    }

    public string BotCommandValue => Button;

    public Task PerformMessageTextBotCommandEqualsAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .SendMessage(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                string.Format(
                    TR.L + "_BACKOFFICE_MAIN_TXT",
                    context.Base.TelegramUser.FirstName.EscapeHtml(),
                    context.Base.TelegramUser.LastName?.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.MainPage, cancellationToken: cancellationToken);
    }
}