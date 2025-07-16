using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Teams;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingAudienceTeamListBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<Team>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingteams</b>.{mailingId}.{mailingPageNumber}.{teamPageNumber}
    /// </summary>
    public const string Button = "mailingteams";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingFilterListBackOfficeContext<Team> context)
    {
        return Keyboards.MailingAudienceTeams(
            context.Entities,
            context.IncludedTeams,
            context.ExcludedTeams,
            context.PageNumber,
            context.PageCount,
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override Task<Team[]> GetEntityPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Teams
            .GetPaginatedAsync(
                pageNumber,
                pageSize,
                cancellationToken);
    }

    protected override Task<int> GetEntityCountAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Teams
            .CountAsync(cancellationToken);
    }
}