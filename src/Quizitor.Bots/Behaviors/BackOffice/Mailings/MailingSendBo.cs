using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingSendBo(
    IDbContextProvider dbContextProvider) :
    MailingProfileBo(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingsend</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public new const string Button = "mailingsend";

    public override string[] Permissions => [UserPermission.BackOfficeMailingSend];

    protected override string ButtonInline => Button;

    public override Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<IMailingProfileBackOfficeContext> context,
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
                    TR.L + "_BACKOFFICE_MAILING_SEND_CONFIRMATION_TXT",
                    context.Base.Mailing.Name.EscapeHtml(),
                    context.Base.Mailing.Text,
                    context.Base.PredictedMessagesCount,
                    context.Base.PredictedUsersCount,
                    context.Base.PredictedBotsCount),
                ParseMode.Html,
                replyMarkup: Keyboards.MailingSend(
                    context.Base.Mailing.Id,
                    context.Base.MailingPageNumber),
                cancellationToken: cancellationToken);
    }
}