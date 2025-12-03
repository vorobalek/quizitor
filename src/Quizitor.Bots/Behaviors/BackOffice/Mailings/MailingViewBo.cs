using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IMailingBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IMailingBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingViewBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IMailingBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>mailing</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public const string Button = "mailing";

    protected virtual string ButtonInternal => Button;

    public override string[] Permissions => [UserPermission.BackOfficeMailingView];

    public string CallbackQueryDataPrefixValue => $"{ButtonInternal}.";

    public virtual Task PerformCallbackQueryDataPrefixAsync(
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
                    TR.L + "_BACKOFFICE_MAILING_VIEW_TXT",
                    context.Base.Mailing.Name.Html,
                    context.Base.Mailing.Text.Html),
                ParseMode.Html,
                replyMarkup: Keyboards.MailingView(
                    context.Base.Mailing.Id,
                    context.Base.MailingPageNumber), cancellationToken: cancellationToken);
    }

    protected override async Task<IMailingBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } mailingIdString,
                {
                } mailingPageNumberString
            ] &&
            int.TryParse(mailingIdString, out var mailingId) &&
            int.TryParse(mailingPageNumberString, out var mailingPageNumber) &&
            await dbContextProvider.Mailings.GetByIdOrDefaultAsync(mailingId, cancellationToken) is { } mailing)
        {
            return IMailingBackOfficeContext.Create(
                mailing,
                mailingPageNumber,
                backOfficeContext);
        }

        return null;
    }
}