using LPlus;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;
using Context = Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix.ICallbackQueryDataPrefixContext<Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure.IMailingBackOfficeContext>;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

internal sealed class MailingPreviewBo(
    IDbContextProvider dbContextProvider) :
    MailingViewBo(
        dbContextProvider)
{
    /// <summary>
    ///     <b>mailingpreview</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public new const string Button = "mailingpreview";

    protected override string ButtonInternal => Button;

    public override Task PerformCallbackQueryDataPrefixAsync(
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
                    TR.L + "_BACKOFFICE_MAILING_PREVIEW_TXT",
                    context.Base.Mailing.Name.EscapeHtml(),
                    context.Base.Mailing.Text),
                ParseMode.Html,
                replyMarkup: Keyboards.MailingPreview(
                    context.Base.Mailing.Id,
                    context.Base.MailingPageNumber), cancellationToken: cancellationToken);
    }
}