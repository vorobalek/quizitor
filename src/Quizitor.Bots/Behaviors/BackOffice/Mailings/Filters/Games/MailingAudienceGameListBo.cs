using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Games;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingAudienceGameListBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<Game>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailinggames</b>.{mailingId}.{mailingPageNumber}.{gamePageNumber}
    /// </summary>
    public const string Button = "mailinggames";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingFilterListBackOfficeContext<Game> context)
    {
        return Keyboards.MailingAudienceGames(
            context.Entities,
            context.IncludedGames,
            context.ExcludedGames,
            context.PageNumber,
            context.PageCount,
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override Task<Game[]> GetEntityPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Games
            .GetPaginatedAsync(
                pageNumber,
                pageSize,
                cancellationToken);
    }

    protected override Task<int> GetEntityCountAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Games
            .CountAsync(cancellationToken);
    }
}