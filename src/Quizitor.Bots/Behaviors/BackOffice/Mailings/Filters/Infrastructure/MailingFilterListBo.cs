using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
internal abstract class MailingFilterListBo<TEntity, TContext>(
    IDbContextProvider dbContextProvider) :
    MailingProfileBo<TContext>(dbContextProvider)
    where TContext : IMailingFilterListBackOfficeContext<TEntity>
{
    private const int PageSize = 10;

    protected async Task<IMailingFilterListBackOfficeContext<TEntity>> PrepareMailingFilterListContextAsync(
        Mailing mailing,
        MailingProfile? mailingProfile,
        IBackOfficeContext backOfficeContext,
        int mailingPageNumber,
        int pageNumber,
        CancellationToken cancellationToken)
    {
        var context = await PrepareMailingProfileContextAsync(
            mailing,
            mailingProfile,
            backOfficeContext,
            mailingPageNumber,
            cancellationToken);

        var entities = await GetEntityPaginatedAsync(
            pageNumber,
            PageSize,
            cancellationToken);

        var gamePageCount = Convert.ToInt32(
            Math.Ceiling(
                Convert.ToDouble(
                    await GetEntityCountAsync(cancellationToken)) / PageSize));

        return IMailingFilterListBackOfficeContext<TEntity>.Create(
            entities,
            pageNumber,
            gamePageCount,
            context);
    }

    protected abstract Task<TEntity[]> GetEntityPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    protected abstract Task<int> GetEntityCountAsync(CancellationToken cancellationToken);
}

// ReSharper disable once ClassNeverInstantiated.Global
internal abstract class MailingFilterListBo<TEntity>(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<TEntity, IMailingFilterListBackOfficeContext<TEntity>>(dbContextProvider)
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override async Task<IMailingFilterListBackOfficeContext<TEntity>?> PrepareContextAsync(
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
                } pageNumberString
            ] &&
            int.TryParse(mailingIdString, out var mailingId) &&
            int.TryParse(mailingPageNumberString, out var mailingPageNumber) &&
            int.TryParse(pageNumberString, out var pageNumber) &&
            await _dbContextProvider.Mailings.GetByIdOrDefaultAsync(mailingId, cancellationToken) is { } mailing)
        {
            var mailingProfile = await _dbContextProvider
                .Mailings
                .GetProfileByMailingIdUserIdOrDefaultAsync(
                    mailing.Id,
                    backOfficeContext.Identity.User.Id,
                    cancellationToken);

            return await PrepareMailingFilterListContextAsync(
                mailing,
                mailingProfile,
                backOfficeContext,
                mailingPageNumber,
                pageNumber,
                cancellationToken);
        }

        return null;
    }
}