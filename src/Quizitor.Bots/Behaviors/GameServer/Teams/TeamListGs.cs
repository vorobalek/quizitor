using LPlus;
using Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<ITeamListGameServerContext>;
using Context = ICallbackQueryDataPrefixContext<ITeamListGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class TeamListGs(
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<ITeamListGameServerContext>(dbContextProvider),
    Behavior
{
    public const int PageSize = 10;

    /// <summary>
    ///     <b>teams</b>.{teamPageNumber}
    /// </summary>
    public const string Button = "teams";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context, cancellationToken);
    }

    public static Task ResponseAsync<TContext>(
        TContext? context,
        CancellationToken cancellationToken)
        where TContext : IBehaviorTraitContext<ITeamListGameServerContext>
    {
        if (context is null) return Task.CompletedTask;

        var text = TR.L + "_GAME_SERVER_TEAMS_TXT";
        var keyboard = Keyboards.TeamList(
            context.Base.Teams,
            context.Base.TeamPageNumber,
            context.Base.TeamPageCount);

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

    protected override async Task<ITeamListGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameServerContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } teamPageNumberString
            ] &&
            int.TryParse(teamPageNumberString, out var teamPageNumber))
        {
            var teams = await _dbContextProvider
                .Teams
                .GetPaginatedByOwnerIdAsync(
                    gameServerContext.Identity.User.Id,
                    teamPageNumber,
                    PageSize,
                    cancellationToken);

            var teamPageCount = Convert.ToInt32(
                Math.Ceiling(
                    Convert.ToDouble(
                        await _dbContextProvider
                            .Teams
                            .CountByOwnerIdAsync(
                                gameServerContext.Identity.User.Id,
                                cancellationToken)) / PageSize));

            return ITeamListGameServerContext.Create(
                teams,
                teamPageNumber,
                teamPageCount,
                gameServerContext);
        }

        return null;
    }
}