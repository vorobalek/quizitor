using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rounds;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRoundListGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IRoundListGameAdminContext>;

internal sealed class RoundListGa(
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IRoundListGameAdminContext>(
        dbContextProvider),
    Behavior
{
    private const int PageSize = 10;
    public const string Button = "rounds";
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions => [UserPermission.GameAdminRoundList];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                TR.L + "_GAME_ADMIN_ROUNDS_TXT",
                ParseMode.Html,
                replyMarkup: Keyboards.RoundList(
                    context.Base.Rounds,
                    context.Base.PageNumber,
                    context.Base.PageCount), cancellationToken: cancellationToken);
    }

    protected override async Task<IRoundListGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameAdminContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } pageNumberString
            ] &&
            int.TryParse(pageNumberString, out var pageNumber))
        {
            var rounds = await _dbContextProvider
                .Rounds
                .GetPaginatedByGameIdAsync(
                    gameAdminContext.Session.GameId,
                    pageNumber,
                    PageSize,
                    cancellationToken);

            var pageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await _dbContextProvider
                            .Rounds
                            .CountByGameIdAsync(
                                gameAdminContext.Session.GameId,
                                cancellationToken)) / PageSize));

            return IRoundListGameAdminContext.Create(
                rounds,
                pageNumber,
                pageCount,
                gameAdminContext);
        }

        return null;
    }
}