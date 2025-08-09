using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.LoadBalancer;
using Quizitor.Bots.UI.LoadBalancer;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors;

internal abstract class GameServerBehaviorBase<TContext>(IDbContextProvider dbContextProvider) :
    BalanceGameServerLb<TContext>(
        dbContextProvider)
    where TContext : ILoadBalancerContext
{
    public sealed override BotType Type => BotType.GameServer;

    protected sealed override bool ShouldPerformInternal(IBehaviorContext baseContext)
    {
        return false;
    }

    protected sealed override string GetRedirectMessage(string targetBotUsername)
    {
        return string.Format(
            TR.L + "_GAME_SERVER_REDIRECT_TXT",
            targetBotUsername.EscapeHtml());
    }

    protected sealed override InlineKeyboardMarkup GetRedirectKeyboard(string targetBotUsername)
    {
        return Keyboards.GameServerRedirect(targetBotUsername);
    }
}