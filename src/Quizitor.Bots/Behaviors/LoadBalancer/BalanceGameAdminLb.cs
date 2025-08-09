using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.Exceptions;
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

internal abstract class BalanceGameAdminLb<TContext>(IDbContextProvider dbContextProvider) :
    LoadBalancerBehavior<TContext>(dbContextProvider)
    where TContext : ILoadBalancerContext
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public sealed override string[] Permissions =>
    [
        UserPermission.LoadBalancerGameAdminAssign,
        .. GameAdminPermissions
    ];

    protected abstract string[] GameAdminPermissions { get; }

    protected sealed override BotType UserTargetBotType => BotType.GameAdmin;

    protected sealed override int? GetUserTargetBotId(User user)
    {
        return user.GameAdminId;
    }

    protected sealed override Task<Bot?> GetTargetBotAsync(CancellationToken cancellationToken)
    {
        return _dbContextProvider
            .Bots
            .GetCandidateGameAdminOrDefaultAsync(cancellationToken);
    }

    protected sealed override async Task<bool> RedirectIfNeededAsync(
        Bot? targetBot,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        if (targetBot is null)
        {
            if (baseContext.Identity.User.GameAdminId is not null)
            {
                baseContext.Identity.User.GameAdminId = null;
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
                            TR.L + "_LOAD_BALANCER_NO_GAME_ADMIN_AVAILABLE_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return true;
        }

        if (baseContext.Identity.User.GameAdminId != targetBot.Id)
        {
            baseContext.Identity.User.GameAdmin = targetBot;
            await _dbContextProvider
                .Users
                .UpdateAsync(
                    baseContext.Identity.User,
                    cancellationToken);
        }

        if (baseContext.Identity.User.GameAdminId != baseContext.EntryBot?.Id)
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

    protected sealed override async Task HandleUnauthorizedInternalAsync(
        UnauthorizedException exception,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        await base.HandleUnauthorizedInternalAsync(
            exception,
            baseContext,
            cancellationToken);

        if (exception.MissingPermissions.Contains(UserPermission.LoadBalancerGameAdminAssign))
        {
            if (baseContext.Identity.User.GameAdminId is not null)
            {
                baseContext.Identity.User.GameAdminId = null;
                await _dbContextProvider
                    .Users
                    .UpdateAsync(
                        baseContext.Identity.User,
                        cancellationToken);
            }
        }
    }
}

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BalanceGameAdminLb(IDbContextProvider dbContextProvider) :
    BalanceGameAdminLb<ILoadBalancerContext>(dbContextProvider),
    Behavior
{
    protected override string[] GameAdminPermissions => [UserPermission.LoadBalancerGameAdminAssign];

    public string BotCommandValue => "admin";

    public Task PerformMessageTextBotCommandEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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
            TR.L + "_LOAD_BALANCER_GAME_ADMIN_REDIRECT_TXT",
            targetBotUsername.EscapeHtml());
    }

    protected override InlineKeyboardMarkup GetRedirectKeyboard(string targetBotUsername)
    {
        return Keyboards.GameAdminRedirect(targetBotUsername);
    }
}