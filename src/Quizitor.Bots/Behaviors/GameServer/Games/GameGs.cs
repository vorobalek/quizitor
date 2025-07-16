using LPlus;
using Quizitor.Bots.Behaviors.GameServer.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Games;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IGameGameServerContext>;
using Context = ICallbackQueryDataEqualsContext<IGameGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class GameGs(IDbContextProvider dbContextProvider) :
    GameServerBehavior<IGameGameServerContext>(dbContextProvider),
    Behavior
{
    public const string Button = "game";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var text = string.Format(
            TR.L + "_GAME_SERVER_GAME_TXT",
            context.Base.Game.Title.EscapeHtml(),
            context.Base.Session.Name.EscapeHtml(),
            context.Base.RoundsCount,
            context.Base.QuestionsCount,
            context.Base.ConnectedUsersCount);

        var keyboard = Keyboards.Game();

        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                text,
                ParseMode.Html,
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
    }

    protected override async Task<IGameGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        var roundsCount = await _dbContextProvider
            .Rounds
            .CountByGameIdAsync(
                gameServerContext.Game.Id,
                cancellationToken);

        var questionsCount = await _dbContextProvider
            .Questions
            .CountByGameIdAsync(
                gameServerContext.Game.Id,
                cancellationToken);

        var connectedUsersCount = await _dbContextProvider
            .Users
            .CountBySessionIdAsync(
                gameServerContext.Session.Id,
                cancellationToken);

        return IGameGameServerContext.Create(
            roundsCount,
            questionsCount,
            connectedUsersCount,
            gameServerContext);
    }
}