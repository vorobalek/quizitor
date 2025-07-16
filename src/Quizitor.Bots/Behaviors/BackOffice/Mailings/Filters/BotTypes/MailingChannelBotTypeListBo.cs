using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.BotTypes;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingChannelBotTypeListBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<BotType>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingbottypes</b>.{mailingId}.{mailingPageNumber}.{botTypePageNumber}
    /// </summary>
    public const string Button = "mailingbottypes";

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingFilterListBackOfficeContext<BotType> context)
    {
        return Keyboards.MailingChannelBotTypes(
            context.Entities,
            context.IncludedBotTypes,
            context.ExcludedBotTypes,
            context.PageNumber,
            context.PageCount,
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override Task<BotType[]> GetEntityPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            Enum.GetValues<BotType>()
                .Skip(pageSize * pageNumber)
                .Take(pageSize)
                .ToArray());
    }

    protected override Task<int> GetEntityCountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Enum.GetValues<BotType>().Length);
    }
}