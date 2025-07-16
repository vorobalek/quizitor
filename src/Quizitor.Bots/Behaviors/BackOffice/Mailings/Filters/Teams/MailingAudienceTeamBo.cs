using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Teams;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingAudienceTeamBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<Team>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingteam</b>.{mailingId}.{mailingPageNumber}.{teamId}.{teamPageNumber}
    /// </summary>
    public const string Button = "mailingteam";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingFilterBackOfficeContext<Team> context)
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

    protected override Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<Team>> context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IncludedTeams.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .ExcludeTeamMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        if (context.Base.ExcludedTeams.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .RemoveTeamMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        return _dbContextProvider
            .Mailings
            .IncludeTeamMailingFilterAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                context.Base.Entity.Id,
                cancellationToken);
    }

    protected override Task<Team?> GetEntityByIdOrDefaultAsync(
        int entityId,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Teams
            .GetByIdOrDefaultAsync(
                entityId,
                cancellationToken);
    }
}