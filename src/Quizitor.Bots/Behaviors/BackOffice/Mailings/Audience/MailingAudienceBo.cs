using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Audience;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingAudienceBo(
    IDbContextProvider dbContextProvider) :
    MailingProfileBo(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingaudience</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public new const string Button = "mailingaudience";

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingProfileBackOfficeContext context)
    {
        return Keyboards.MailingAudience(
            context.Mailing.Id,
            context.MailingPageNumber);
    }
}