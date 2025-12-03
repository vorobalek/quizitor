using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Common;
using Quizitor.Data;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Teams;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IGameServerContext>;
using Context = ICallbackQueryDataEqualsContext<IGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal class TeamGs(IDbContextProvider dbContextProvider) :
    GameServerBehavior(dbContextProvider),
    Behavior
{
    public const string Button = "team";

    public override string[] Permissions => [];

    protected virtual string ButtonInternal => Button;

    public string CallbackQueryDataValue => ButtonInternal;

    public virtual Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return ResponseAsync(context.Base, context.MessageId, cancellationToken);
    }

    protected static Task ResponseAsync(
        IGameServerContext context,
        int? messageId,
        CancellationToken cancellationToken)
    {
        if (context.SessionTeamInfo is not { } teamInfo)
        {
            return MainPageGs.ResponseAsync(context, messageId, cancellationToken);
        }

        var text = string.Format(
            TR.L + "_GAME_SERVER_TEAM_TXT",
            teamInfo.Team.Name.Html,
            teamInfo.Leader is not { } leader
                ? TR.L + "_GAME_SERVER_TEAM_LEADER_EMPTY_TXT"
                : string.Format(
                    leader.Id == context.Identity.User.Id
                        ? TR.L + "_GAME_SERVER_TEAM_LEADER_YOU_TXT"
                        : TR.L + "_GAME_SERVER_TEAM_LEADER_TXT",
                    leader.Id,
                    leader.GetFullName().Html),
            string.Format(
                TR.L + "_GAME_SERVER_TEAM_MEMBERS_TXT",
                string.Join(
                    Environment.NewLine,
                    teamInfo.Members.Select(x =>
                        string.Format(
                            x.Id == context.Identity.User.Id
                                ? TR.L + "_GAME_SERVER_TEAM_MEMBER_ITEM_YOU_TXT"
                                : TR.L + "_GAME_SERVER_TEAM_MEMBER_ITEM_TXT",
                            x.Id,
                            x.GetFullName().Html)))),
            string.Format(
                TR.L + "_GAME_SERVER_TEAM_OFFLINE_MEMBERS_TXT",
                teamInfo.OfflineMembers.Length > 0
                    ? string.Join(
                        Environment.NewLine,
                        teamInfo.OfflineMembers.Select(x =>
                            string.Format(
                                TR.L + "_GAME_SERVER_TEAM_OFFLINE_MEMBER_ITEM_TXT",
                                x.Id,
                                x.GetFullName().Html)))
                    : TR.L + "_SHARED_NO_TXT"));
        var keyboard = Keyboards.Team(teamInfo.Leader?.Id != context.Identity.User.Id);

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