using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IDeleteMailingConfirmBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IDeleteMailingConfirmBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DeleteMailingConfirmBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IDeleteMailingConfirmBackOfficeContext>,
    Behavior
{
    /// <summary>
    ///     <b>deletemailingconfirm</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public const string Button = "deletemailingconfirm";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeMailingList,
        UserPermission.BackOfficeMailingDelete
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        await dbContextProvider
            .Mailings
            .RemoveAsync(
                context.Base.Mailing,
                cancellationToken);

        dbContextProvider
            .AddPostCommitTask(async () =>
            {
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string
                            .Format(
                                TR.L + "_BACKOFFICE_MAILING_DELETED_CLB",
                                context.Base.Mailing.Name.Html),
                        true,
                        cancellationToken: cancellationToken);

                await MailingListBo.ResponseAsync(
                    context,
                    cancellationToken);
            });
    }

    protected override async Task<IDeleteMailingConfirmBackOfficeContext?> PrepareContextAsync(
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
            var mailings = await dbContextProvider
                .Mailings
                .GetPaginatedAfterDeletionAsync(
                    mailing.Id,
                    mailingPageNumber,
                    MailingListBo.PageSize,
                    cancellationToken);

            var mailingsCount = await dbContextProvider
                .Mailings
                .CountAsync(cancellationToken) - 1;

            var mailingPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(mailingsCount) / MailingListBo.PageSize));

            return IDeleteMailingConfirmBackOfficeContext.Create(
                mailing,
                mailings,
                mailingsCount,
                mailingPageNumber,
                mailingPageCount,
                backOfficeContext);
        }

        return null;
    }
}