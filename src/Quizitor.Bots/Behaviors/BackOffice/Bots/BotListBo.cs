using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

using Context = ICallbackQueryDataPrefixContext<IBotListBackOfficeContext>;

internal abstract class BotListBo<TContext>(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<TContext>,
    ICallbackQueryDataPrefixBehaviorTrait<TContext>
    where TContext : IBotListBackOfficeContext
{
    /// <summary>
    ///     <b>bots</b>.{botPageNumber}<br />
    ///     <b>bots</b>.{botPageNumber}.[start|stop].{botId}
    /// </summary>
    public const string Button = "bots";

    private const int PageSize = 10;

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public abstract Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
        CancellationToken cancellationToken);

    protected async Task<IBotListBackOfficeContext> PrepareBaseAsync(
        IBackOfficeContext backOfficeContext,
        int botPageNumber,
        CancellationToken cancellationToken)
    {
        var bots = await dbContextProvider
            .Bots
            .GetPaginatedAsync(
                botPageNumber,
                PageSize,
                cancellationToken);

        var botPageCount = Convert.ToInt32(
            Math.Ceiling(
                Convert.ToDouble(
                    await dbContextProvider
                        .Bots
                        .CountAsync(cancellationToken)) / PageSize));

        return IBotListBackOfficeContext.Create(
            bots,
            botPageNumber,
            botPageCount,
            backOfficeContext);
    }

    protected static Task ResponseAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                TR.L + "_BACKOFFICE_BOTS_TXT",
                ParseMode.Html,
                replyMarkup: Keyboards.BotList(
                    context.Base.Bots,
                    context.Base.BotPageNumber,
                    context.Base.BotPageCount), cancellationToken: cancellationToken);
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BotListBo(
    IDbContextProvider dbContextProvider) :
    BotListBo<IBotListBackOfficeContext>(
        dbContextProvider)
{
    public override string[] Permissions => [UserPermission.BackOfficeBotList];

    protected override async Task<IBotListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } botPageNumberString
            ] &&
            int.TryParse(botPageNumberString, out var botPageNumber))
        {
            var botListContext = await PrepareBaseAsync(
                backOfficeContext,
                botPageNumber,
                cancellationToken);

            return botListContext;
        }

        return null;
    }

    public override Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }
}