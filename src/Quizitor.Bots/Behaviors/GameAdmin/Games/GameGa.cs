using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Games.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Games;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IGameGameAdminContext>;
using Context = ICallbackQueryDataEqualsContext<IGameGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class GameGa(IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IGameGameAdminContext>(dbContextProvider),
    Behavior
{
    public const string Button = "game";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected virtual string ButtonInternal => Button;

    protected override string[] GameAdminPermissions => [UserPermission.GameAdminGameView];

    public string CallbackQueryDataValue => ButtonInternal;

    public virtual Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var text = string.Format(
            TR.L + "_GAME_ADMIN_GAME_TXT",
            context.Base.Game.Title.Html,
            context.Base.Session.Name.Html,
            context.Base.RoundsCount,
            context.Base.QuestionsCount,
            context.Base.SubmissionsCount,
            context.Base.ConnectedUsersCount);

        var keyboard = Keyboards.Game(context.Base.Session.SyncRating);

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

    protected override async Task<IGameGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var roundsCount = await _dbContextProvider
            .Rounds
            .CountByGameIdAsync(
                gameAdminContext.Game.Id,
                cancellationToken);

        var questionsCount = await _dbContextProvider
            .Questions
            .CountByGameIdAsync(
                gameAdminContext.Game.Id,
                cancellationToken);

        var submissionsCount = await _dbContextProvider
            .Submissions
            .CountBySessionIdAsync(
                gameAdminContext.Session.Id,
                cancellationToken);

        var connectedUsersCount = await _dbContextProvider
            .Users
            .CountBySessionIdAsync(
                gameAdminContext.Session.Id,
                cancellationToken);

        return IGameGameAdminContext.Create(
            roundsCount,
            questionsCount,
            submissionsCount,
            connectedUsersCount,
            gameAdminContext);
    }
}