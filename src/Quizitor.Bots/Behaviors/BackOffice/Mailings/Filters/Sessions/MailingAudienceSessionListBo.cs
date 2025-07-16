using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Sessions;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingAudienceSessionListBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<Session>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingsessions</b>.{mailingId}.{mailingPageNumber}.{sessionPageNumber}
    /// </summary>
    public const string Button = "mailingsessions";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingFilterListBackOfficeContext<Session> context)
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
}