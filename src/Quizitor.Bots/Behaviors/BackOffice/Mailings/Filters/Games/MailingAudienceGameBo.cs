using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Games;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingAudienceGameBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<Game>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailinggame</b>.{mailingId}.{mailingPageNumber}.{gameId}.{gamePageNumber}
    /// </summary>
    public const string Button = "mailinggame";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingFilterBackOfficeContext<Game> context)
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

    protected override Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<Game>> context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IncludedGames.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .ExcludeGameMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        if (context.Base.ExcludedGames.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .RemoveGameMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        return _dbContextProvider
            .Mailings
            .IncludeGameMailingFilterAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                context.Base.Entity.Id,
                cancellationToken);
    }

    protected override Task<Game?> GetEntityByIdOrDefaultAsync(
        int entityId,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Games
            .GetByIdOrDefaultAsync(
                entityId,
                cancellationToken);
    }
}