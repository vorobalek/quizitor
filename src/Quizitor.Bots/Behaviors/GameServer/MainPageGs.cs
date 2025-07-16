using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer;

using CallbackBehavior = ICallbackQueryDataEqualsBehaviorTrait<IGameServerContext>;
using CallbackContext = ICallbackQueryDataEqualsContext<IGameServerContext>;
using BotCommandBehavior = IMessageTextBotCommandEqualsBehaviorTrait<IGameServerContext>;
using BotCommandContext = IMessageTextBotCommandEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MainPageGs(IDbContextProvider dbContextProvider) :
    GameServerBehavior(dbContextProvider),
    CallbackBehavior,
    BotCommandBehavior
{
    public const string Button = "start";

    public override string[] Permissions => [];

    public string BotCommandValue => Button;

    public Task PerformMessageTextBotCommandEqualsAsync(
        BotCommandContext context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context.Base, null, cancellationToken);
    }

    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        CallbackContext context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context.Base, context.MessageId, cancellationToken);
    }

    public static Task ResponseAsync(
        IGameServerContext context,
        int? messageId,
        CancellationToken cancellationToken)
    {
        var text = string.Format(
            TR.L + "_GAME_SERVER_MAIN_TXT",
            context.Identity.User.FirstName.EscapeHtml(),
            context.Identity.User.LastName?.EscapeHtml(),
            string.Format(
                TR.L + "_GAME_SERVER_MAIN_GAME_INFO_TXT",
                context.Game.Title.EscapeHtml(),
                context.Session.Name.EscapeHtml()),
            context.SessionTeamInfo is not { } teamInfo
                ? string.Format(
                    TR.L + "_GAME_SERVER_MAIN_WITHOUT_TEAM_INFO_TXT",
                    TR.L + "_GAME_SERVER_TEAMS_BTN")
                : string.Format(
                    TR.L + "_GAME_SERVER_MAIN_TEAM_INFO_TXT",
                    teamInfo.Team.Name.EscapeHtml()));

        var keyboard = Keyboards.MainPage(context.SessionTeamInfo is not null);

        return messageId.HasValue
            ? context
                .Client
                .EditMessageText(
                    context.UpdateContext,
                    context.TelegramUser.Id,
                    messageId.Value,
                    text,
                    ParseMode.Html,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken)
            : context
                .Client
                .SendMessage(
                    context.UpdateContext,
                    context.TelegramUser.Id,
                    text,
                    ParseMode.Html,
                    replyMarkup: keyboard,
                    cancellationToken: cancellationToken);
    }
}