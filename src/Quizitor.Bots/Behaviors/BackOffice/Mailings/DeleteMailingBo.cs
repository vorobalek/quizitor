using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IDeleteMailingBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IDeleteMailingBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DeleteMailingBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IDeleteMailingBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>deletemailing</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public const string Button = "deletemailing";

    public override string[] Permissions => [UserPermission.BackOfficeMailingDelete];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
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
                    TR.L + "_BACKOFFICE_DELETE_MAILING_CONFIRMATION_TXT",
                    context.Base.Mailing.Name.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.DeleteMailing(
                    context.Base.Mailing.Id,
                    context.Base.MailingPageNumber),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IDeleteMailingBackOfficeContext?> PrepareContextAsync(
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
            return IDeleteMailingBackOfficeContext.Create(
                mailing,
                mailingPageNumber,
                backOfficeContext);
        }

        return null;
    }
}