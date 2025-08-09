using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.UI.LoadBalancer;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.LoadBalancer;

using Behavior = IMessageTextBotCommandEqualsBehaviorTrait<ILoadBalancerContext>;
using Context = IMessageTextBotCommandEqualsContext<ILoadBalancerContext>;

internal abstract class BalanceGameServerLb<TContext>(IDbContextProvider dbContextProvider) :
    LoadBalancerBehavior<TContext>(dbContextProvider)
    where TContext : ILoadBalancerContext
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected sealed override BotType UserTargetBotType => BotType.GameServer;

    protected sealed override int? GetUserTargetBotId(User user)
    {
        return user.GameServerId;
    }

    protected sealed override Task<Bot?> GetTargetBotAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Bots
            .GetCandidateGameServerOrDefaultAsync(cancellationToken);
    }

    protected sealed override async Task<bool> RedirectIfNeededAsync(
        Bot? targetBot,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        if (targetBot is null)
        {
            if (baseContext.Identity.User.GameServerId is not null)
            {
                baseContext.Identity.User.GameServerId = null;
                await _dbContextProvider
                    .Users
                    .UpdateAsync(
                        baseContext.Identity.User,
                        cancellationToken);
            }

            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await baseContext
                        .Client
                        .SendMessage(
                            baseContext.UpdateContext,
                            baseContext.TelegramUser.Id,
                            TR.L + "_LOAD_BALANCER_NO_GAME_SERVER_AVAILABLE_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return true;
        }

        if (baseContext.Identity.User.GameServerId != targetBot.Id)
        {
            baseContext.Identity.User.GameServer = targetBot;
            await _dbContextProvider
                .Users
                .UpdateAsync(
                    baseContext.Identity.User,
                    cancellationToken);
        }

        if (baseContext.Identity.User.GameServerId != baseContext.EntryBot?.Id)
        {
            var targetBotUsername = targetBot.Username!;
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await baseContext
                        .Client
                        .SendMessage(
                            baseContext.UpdateContext,
                            baseContext.TelegramUser.Id,
                            GetRedirectMessage(targetBotUsername),
                            ParseMode.Html,
                            replyMarkup: GetRedirectKeyboard(targetBotUsername),
                            cancellationToken: cancellationToken));
            return true;
        }

        return false;
    }

    protected abstract string GetRedirectMessage(string targetBotUsername);

    protected abstract InlineKeyboardMarkup GetRedirectKeyboard(string targetBotUsername);
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BalanceGameServerLb(
    BalanceGameAdminLb balanceGameAdminLb,
    IDbContextProvider dbContextProvider) :
    BalanceGameServerLb<ILoadBalancerContext>(dbContextProvider),
    Behavior
{
    public override string[] Permissions => [];

    public string BotCommandValue => "start";

    public Task PerformMessageTextBotCommandEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override bool ShouldPerformInternal(IBehaviorContext baseContext)
    {
        return !balanceGameAdminLb.ShouldPerform(baseContext) &&
               !ShouldPerformMessageTextBotCommandEquals(baseContext);
    }

    protected override Task<ILoadBalancerContext?> PrepareLoadBalancerInternalAsync(
        Bot? targetBot,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<ILoadBalancerContext?>(ILoadBalancerContext.Create(targetBot, baseContext));
    }

    protected override string GetRedirectMessage(string targetBotUsername)
    {
        return string.Format(
            TR.L + "_LOAD_BALANCER_GAME_SERVER_REDIRECT_TXT",
            targetBotUsername.EscapeHtml());
    }

    protected override InlineKeyboardMarkup GetRedirectKeyboard(string targetBotUsername)
    {
        return Keyboards.GameServerRedirect(targetBotUsername);
    }
}