using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRoundListBackOfficeContext>;
using Context = ICallbackQueryDataPrefixContext<IRoundListBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RoundListBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IRoundListBackOfficeContext>,
    Behavior
{
    public const int PageSize = 10;

    /// <summary>
    ///     <b>rounds</b>.{gameId}.{gamePageNumber}.{roundPageNumber}
    /// </summary>
    public const string Button = "rounds";

    public override string[] Permissions => [UserPermission.BackOfficeRoundList];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    protected override async Task<IRoundListBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } gameIdString,
                {
                } gamePageNumberString,
                {
                } roundPageNumberString
            ] &&
            int.TryParse(gameIdString, out var gameId) &&
            int.TryParse(gamePageNumberString, out var gamePageNumber) &&
            int.TryParse(roundPageNumberString, out var roundPageNumber) &&
            await dbContextProvider.Games.GetByIdOrDefaultAsync(gameId, cancellationToken) is { } game)
        {
            var rounds = await dbContextProvider
                .Rounds
                .GetPaginatedByGameIdAsync(
                    gameId,
                    roundPageNumber,
                    PageSize,
                    cancellationToken);

            var roundPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await dbContextProvider
                            .Rounds
                            .CountByGameIdAsync(
                                gameId,
                                cancellationToken)) / PageSize));

            return IRoundListBackOfficeContext.Create(
                game,
                rounds,
                gamePageNumber,
                roundPageNumber,
                roundPageCount,
                backOfficeContext);
        }

        return null;
    }

    public static Task ResponseAsync<TContext>(
        TContext? context,
        CancellationToken cancellationToken)
        where TContext : IBehaviorTraitContext<IRoundListBackOfficeContext>
    {
        if (context is null) return Task.CompletedTask;

        var text = string
            .Format(
                TR.L + "_BACKOFFICE_ROUNDS_TXT",
                context.Base.Game.Title.EscapeHtml());
        var keyboard = Keyboards.RoundList(
            context.Base.Rounds,
            context.Base.Game.Id,
            context.Base.GamePageNumber,
            context.Base.RoundPageNumber,
            context.Base.RoundPageCount);

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