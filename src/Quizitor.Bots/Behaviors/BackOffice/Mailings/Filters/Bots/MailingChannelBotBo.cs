using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Bots;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingChannelBotBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<Bot>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingbot</b>.{mailingId}.{mailingPageNumber}.{botId}.{botPageNumber}
    /// </summary>
    public const string Button = "mailingbot";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingFilterBackOfficeContext<Bot> context)
    {
        return Keyboards.MailingChannelBots(
            context.Entities,
            context.IncludedBots,
            context.ExcludedBots,
            context.PageNumber,
            context.PageCount,
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override Task<Bot[]> GetEntityPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Bots
            .GetPaginatedAsync(
                pageNumber,
                pageSize,
                cancellationToken);
    }

    protected override Task<int> GetEntityCountAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Bots
            .CountAsync(cancellationToken);
    }

    protected override Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<Bot>> context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IncludedBots.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .ExcludeBotMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        if (context.Base.ExcludedBots.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .RemoveBotMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        return _dbContextProvider
            .Mailings
            .IncludeBotMailingFilterAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                context.Base.Entity.Id,
                cancellationToken);
    }

    protected override Task<Bot?> GetEntityByIdOrDefaultAsync(
        int entityId,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Bots
            .GetByIdOrDefaultAsync(
                entityId,
                cancellationToken);
    }
}