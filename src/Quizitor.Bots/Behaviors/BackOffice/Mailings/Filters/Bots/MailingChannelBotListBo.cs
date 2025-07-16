using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Bots;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingChannelBotListBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<Bot>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingbots</b>.{mailingId}.{mailingPageNumber}.{botPageNumber}
    /// </summary>
    public const string Button = "mailingbots";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingFilterListBackOfficeContext<Bot> context)
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
}