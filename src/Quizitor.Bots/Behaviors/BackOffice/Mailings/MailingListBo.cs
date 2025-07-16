using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IMailingListBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IMailingListBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MailingListBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IMailingListBackOfficeContext>,
    Behavior
{
    public const int PageSize = 10;

    /// <summary>
    ///     <b>mailings</b>.{mailingPageNumber}
    /// </summary>
    public const string Button = "mailings";

    public override string[] Permissions => [UserPermission.BackOfficeMailingList];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<IMailingListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } mailingPageNumberString
            ] &&
            int.TryParse(mailingPageNumberString, out var mailingPageNumber))
        {
            var mailings = await dbContextProvider
                .Mailings
                .GetPaginatedAsync(
                    mailingPageNumber,
                    PageSize,
                    cancellationToken);

            var mailingPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await dbContextProvider
                            .Mailings
                            .CountAsync(cancellationToken)) / PageSize));

            return IMailingListBackOfficeContext.Create(
                mailings,
                mailingPageNumber,
                mailingPageCount,
                backOfficeContext);
        }

        return null;
    }

    public static Task ResponseAsync<TContext>(
        TContext? context,
        CancellationToken cancellationToken)
        where TContext : IBehaviorTraitContext<IMailingListBackOfficeContext>
    {
        if (context is null) return Task.CompletedTask;

        var text = TR.L + "_BACKOFFICE_MAILINGS_TXT";
        var keyboard = Keyboards.MailingList(
            context.Base.Mailings,
            context.Base.MailingPageNumber,
            context.Base.MailingPageCount);

        return context switch
        {
            ICallbackQueryDataPrefixContext callbackQueryDataPrefixContext =>
                context
                    .Base
                    .Client
                    .EditMessageText(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        callbackQueryDataPrefixContext.MessageId,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            IMessageTextContext =>
                context
                    .Base
                    .Client
                    .SendMessage(
                        context.Base.UpdateContext,
                        context.Base.TelegramUser.Id,
                        text,
                        ParseMode.Html,
                        replyMarkup: keyboard,
                        cancellationToken: cancellationToken),
            _ => Task.CompletedTask
        };
    }
}