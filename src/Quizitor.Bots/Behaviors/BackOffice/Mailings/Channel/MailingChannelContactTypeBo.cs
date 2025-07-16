using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Channel;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingChannelContactTypeBo(IDbContextProvider dbContextProvider) : MailingChannelBo(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingcontacttype</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public new const string Button = "mailingcontacttype";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingProfileBackOfficeContext context)
    {
        return Keyboards.MailingChannel(
            context.Mailing.Id,
            context.MailingPageNumber,
            context.ContactType);
    }

    public override async Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<IMailingProfileBackOfficeContext> context,
        CancellationToken cancellationToken)
    {
        var nextValue = (MailingProfileContactType)((Convert.ToInt32(context.Base.ContactType) + 1) % Enum.GetValues<MailingProfileContactType>().Length);
        await _dbContextProvider
            .Mailings
            .SetMailingProfileContactTypeAsync(
                context.Base.Mailing.Id,
                context.Base.Identity.User.Id,
                nextValue,
                cancellationToken);
        _dbContextProvider.AddPostCommitTask(async () =>
        {
            var mailingProfile = await _dbContextProvider
                .Mailings
                .GetProfileByMailingIdUserIdOrDefaultAsync(
                    context.Base.Mailing.Id,
                    context.Base.Identity.User.Id,
                    cancellationToken);

            var newContext = ICallbackQueryDataPrefixContext.Create(
                await PrepareMailingProfileContextAsync(
                    context.Base.Mailing,
                    mailingProfile,
                    context.Base,
                    context.Base.MailingPageNumber,
                    cancellationToken),
                CallbackQueryDataPrefixValue);

            if (newContext is not null)
                await base.PerformCallbackQueryDataPrefixAsync(
                    newContext,
                    cancellationToken);
        });
    }
}