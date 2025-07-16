using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Channel;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingChannelBo(
    IDbContextProvider dbContextProvider) :
    MailingProfileBo(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingchannel</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public new const string Button = "mailingchannel";

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingProfileBackOfficeContext context)
    {
        return Keyboards.MailingChannel(
            context.Mailing.Id,
            context.MailingPageNumber,
            context.ContactType);
    }
}