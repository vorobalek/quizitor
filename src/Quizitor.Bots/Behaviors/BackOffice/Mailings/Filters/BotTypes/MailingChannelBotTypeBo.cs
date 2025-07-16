using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.BotTypes;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingChannelBotTypeBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<BotType, BotType>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingbottype</b>.{mailingId}.{mailingPageNumber}.{botId}.{botTypePageNumber}
    /// </summary>
    public const string Button = "mailingbottype";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingFilterBackOfficeContext<BotType> context)
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

    protected override Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<BotType>> context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IncludedBotTypes.Any(x => x == context.Base.Entity))
        {
            return _dbContextProvider
                .Mailings
                .ExcludeBotTypeMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity,
                    cancellationToken);
        }

        if (context.Base.ExcludedBotTypes.Any(x => x == context.Base.Entity))
        {
            return _dbContextProvider
                .Mailings
                .RemoveBotTypeMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity,
                    cancellationToken);
        }

        return _dbContextProvider
            .Mailings
            .IncludeBotTypeMailingFilterAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                context.Base.Entity,
                cancellationToken);
    }

    protected override bool TryParseEntityId(string entityIdString, out BotType entityId)
    {
        return Enum.TryParse(entityIdString, out entityId);
    }

    protected override Task<BotType> GetEntityByIdOrDefaultAsync(BotType entityId, CancellationToken cancellationToken)
    {
        return Task.FromResult(entityId);
    }
}