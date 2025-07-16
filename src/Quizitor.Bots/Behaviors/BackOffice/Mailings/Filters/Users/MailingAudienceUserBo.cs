using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Users;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingAudienceUserBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterBo<User, long>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailinguser</b>.{mailingId}.{mailingPageNumber}.{userId}.{userPageNumber}
    /// </summary>
    public const string Button = "mailinguser";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingFilterBackOfficeContext<User> context)
    {
        return Keyboards.MailingAudienceUsers(
            context.Entities,
            context.IncludedUsers,
            context.ExcludedUsers,
            context.PageNumber,
            context.PageCount,
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override Task<User[]> GetEntityPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Users
            .GetPaginatedAsync(
                pageNumber,
                pageSize,
                cancellationToken);
    }

    protected override Task<int> GetEntityCountAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Users
            .CountAsync(cancellationToken);
    }

    protected override Task SetFilterAsync(
        ICallbackQueryDataPrefixContext<IMailingFilterBackOfficeContext<User>> context,
        CancellationToken cancellationToken)
    {
        if (context.Base.IncludedUsers.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .ExcludeUserMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        if (context.Base.ExcludedUsers.Any(x => x.Id == context.Base.Entity.Id))
        {
            return _dbContextProvider
                .Mailings
                .RemoveUserMailingFilterAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    context.Base.Entity.Id,
                    cancellationToken);
        }

        return _dbContextProvider
            .Mailings
            .IncludeUserMailingFilterAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                context.Base.Entity.Id,
                cancellationToken);
    }

    protected override bool TryParseEntityId(string entityIdString, out long entityId)
    {
        return long.TryParse(entityIdString, out entityId);
    }

    protected override Task<User?> GetEntityByIdOrDefaultAsync(
        long entityId,
        CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Users
            .GetByIdOrDefaultAsync(
                entityId,
                cancellationToken);
    }
}