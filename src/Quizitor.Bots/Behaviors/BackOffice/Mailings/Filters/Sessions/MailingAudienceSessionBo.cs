using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Sessions;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingAudienceSessionBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<Session>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingsession</b>.{mailingId}.{mailingPageNumber}.{sessionId}.{sessionPageNumber}
    /// </summary>
    public const string Button = "mailingsession";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingFilterBackOfficeContext<Session> context)
    {
        return Keyboards.MailingAudienceSessions(
            context.Entities,
            context.IncludedSessions,
            context.ExcludedSessions,
            context.PageNumber,
            context.PageCount,
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override Task<Session[]> GetEntityPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Sessions
            .GetPaginatedAsync(
                pageNumber,
                pageSize,
                cancellationToken);
    }

    protected override Task<int> GetEntityCountAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Sessions
            .CountAsync(cancellationToken);
    }

    protected override Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<Session>> context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IncludedSessions.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .ExcludeSessionMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        if (context.Base.ExcludedSessions.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .RemoveSessionMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        return _dbContextProvider
            .Mailings
            .IncludeSessionMailingFilterAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                context.Base.Entity.Id,
                cancellationToken);
    }

    protected override Task<Session?> GetEntityByIdOrDefaultAsync(
        int entityId,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Sessions
            .GetByIdOrDefaultAsync(
                entityId,
                cancellationToken);
    }
}