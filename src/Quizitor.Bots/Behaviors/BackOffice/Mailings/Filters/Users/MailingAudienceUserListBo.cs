using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Users;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingAudienceUserListBo(
    IDbContextProvider dbContextProvider) :
    MailingFilterListBo<User>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingusers</b>.{mailingId}.{mailingPageNumber}.{userPageNumber}
    /// </summary>
    public const string Button = "mailingusers";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(
        IMailingFilterListBackOfficeContext<User> context)
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
}