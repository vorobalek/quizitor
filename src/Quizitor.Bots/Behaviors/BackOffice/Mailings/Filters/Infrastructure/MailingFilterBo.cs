using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
internal abstract class MailingFilterBo<TEntity, TKey>(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<TEntity, IMailingFilterBackOfficeContext<TEntity>>(dbContextProvider)
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected abstract Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<TEntity>> context,
        CancellationToken cancellationToken);

    public override async Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<TEntity>> context,
        CancellationToken cancellationToken)
    {
        await SetFilterAsync(context, cancellationToken);
        _dbContextProvider.AddPostCommitTask(async () =>
        {
            var mailingProfile = await _dbContextProvider
                .Mailings
                .GetProfileByMailingIdUserIdOrDefaultAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    cancellationToken);

            var newContext = ICallbackQueryDataPrefixContext.Create(
                IMailingFilterBackOfficeContext<TEntity>.Create(
                    context.Base.Entity,
                    await PrepareMailingFilterListContextAsync(
                        context.Base.Mailing,
                        mailingProfile,
                        context.Base,
                        context.Base.MailingPageNumber,
                        context.Base.PageNumber,
                        cancellationToken)),
                CallbackQueryDataPrefixValue);

            if (newContext is not null)
                await base.PerformCallbackQueryDataPrefixAsync(
                    newContext,
                    cancellationToken);
        });
    }

    protected abstract bool TryParseEntityId(string entityIdString, out TKey entityId);

    protected abstract Task<TEntity?> GetEntityByIdOrDefaultAsync(
        TKey entityId,
        CancellationToken cancellationToken);

    protected override async Task<IMailingFilterBackOfficeContext<TEntity>?> PrepareContextAsync(
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
                } entityIdString,
                {
                } pageNumberString
            ] &&
            int.TryParse(mailingIdString, out var mailingId) &&
            int.TryParse(mailingPageNumberString, out var mailingPageNumber) &&
            TryParseEntityId(entityIdString, out var entityId) &&
            int.TryParse(pageNumberString, out var pageNumber) &&
            await _dbContextProvider.Mailings.GetByIdOrDefaultAsync(mailingId, cancellationToken) is { } mailing &&
            await GetEntityByIdOrDefaultAsync(entityId, cancellationToken) is { } entity)
        {
            var mailingProfile = await _dbContextProvider
                .Mailings
                .GetProfileByMailingIdUserIdOrDefaultAsync(
                    mailing.Id,
                    backOfficeContext.Identity.User.Id,
                    cancellationToken);

            var context = await PrepareMailingFilterListContextAsync(
                mailing,
                mailingProfile,
                backOfficeContext,
                mailingPageNumber,
                pageNumber,
                cancellationToken);

            return IMailingFilterBackOfficeContext<TEntity>.Create(
                entity,
                context);
        }

        return null;
    }
}

internal abstract class MailingFilterBo<TEntity>(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<TEntity, int>(dbContextProvider)
{
    protected override bool TryParseEntityId(string entityIdString, out int entityId)
    {
        return int.TryParse(entityIdString, out entityId);
    }
}