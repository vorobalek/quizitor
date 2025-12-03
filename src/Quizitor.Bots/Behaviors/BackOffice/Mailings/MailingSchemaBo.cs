using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingSchemaBo(IDbContextProvider dbContextProvider) :
    MailingProfileBo<IMailingPreviewSchemaBackofficeContext>(dbContextProvider)
{
    private const int PageSize = 10;

    /// <summary>
    ///     <b>mailingprofile</b>.{mailingId}.{mailingPageNumber}.{schemaPageNumber}
    /// </summary>
    public const string Button = "mailingschema";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    public override string[] Permissions => [UserPermission.BackOfficeMailingView];

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingPreviewSchemaBackofficeContext context)
    {
        throw new NotImplementedException();
    }

    public override Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<IMailingPreviewSchemaBackofficeContext> context,
        CancellationToken cancellationToken)
    {
        var page = context
            .Base
            .Schema
            .Skip(context.Base.SchemaPageNumber * PageSize)
            .Take(PageSize);

        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                string.Format(
                    TR.L + "_BACKOFFICE_MAILING_SCHEMA_TXT",
                    context.Base.Mailing.Name.Html,
                    string.Join(
                        Environment.NewLine,
                        page.Select(x =>
                            string.Format(
                                TR.L + "_BACKOFFICE_MAILING_SCHEMA_USER_TXT",
                                x.Key.Id,
                                x.Key.GetFullName().Html,
                                string.Join(
                                    TR.L + "_BACKOFFICE_MAILING_SCHEMA_SEPARATOR_TXT",
                                    x.Select(e =>
                                        string.Format(
                                            TR.L + "_BACKOFFICE_MAILING_SCHEMA_USER_BOT_TXT",
                                            (e.Username ?? e.Name).Html))))))),
                ParseMode.Html,
                replyMarkup: Keyboards.MailingSchema(
                    context.Base.Mailing.Id,
                    context.Base.MailingPageNumber,
                    context.Base.SchemaPageNumber,
                    context.Base.SchemaPageCount),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IMailingPreviewSchemaBackofficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } mailingIdString,
                {
                } mailingPageNumberString,
                {
                } schemaPageNumberString
            ] &&
            int.TryParse(mailingIdString, out var mailingId) &&
            int.TryParse(mailingPageNumberString, out var mailingPageNumber) &&
            int.TryParse(schemaPageNumberString, out var schemaPageNumber) &&
            await _dbContextProvider.Mailings.GetByIdOrDefaultAsync(mailingId, cancellationToken) is { } mailing)
        {
            var mailingProfile = await _dbContextProvider
                .Mailings
                .GetProfileByMailingIdUserIdOrDefaultAsync(
                    mailing.Id,
                    backOfficeContext.Identity.User.Id,
                    cancellationToken);

            var context = await PrepareMailingProfileContextAsync(
                mailing,
                mailingProfile,
                backOfficeContext,
                mailingPageNumber,
                cancellationToken);

            var pageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(context.Schema.Length) / PageSize));

            return IMailingPreviewSchemaBackofficeContext.Create(
                schemaPageNumber,
                pageCount,
                context);
        }

        return null;
    }
}