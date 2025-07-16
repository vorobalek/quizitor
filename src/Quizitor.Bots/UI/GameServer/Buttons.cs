using LPlus;
using Quizitor.Bots.Behaviors.GameServer;
using Quizitor.Bots.Behaviors.GameServer.Games;
using Quizitor.Bots.Behaviors.GameServer.Rating;
using Quizitor.Bots.Behaviors.GameServer.Sessions;
using Quizitor.Bots.Behaviors.GameServer.Teams;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.GameServer;

internal static class Buttons
{
    public static InlineKeyboardButton Main =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_MAIN_BTN",
            MainPageGs.Button);

    public static InlineKeyboardButton Team =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAM_BTN",
            TeamGs.Button);

    public static InlineKeyboardButton TeamList =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAMS_BTN",
            $"{TeamListGs.Button}.0");

    public static InlineKeyboardButton TeamQr =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAM_QR_BTN",
            TeamQrGs.Button);

    public static InlineKeyboardButton TeamSetLeader =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAM_SET_LEADER_BTN",
            TeamSetLeaderGs.Button);

    public static InlineKeyboardButton TeamUnsetLeader =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAM_UNSET_LEADER_BTN",
            TeamUnsetLeaderGs.Button);

    public static InlineKeyboardButton TeamLeave =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAM_LEAVE_BTN",
            TeamLeaveGs.Button);

    public static InlineKeyboardButton Game =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_GAME_BTN",
            GameGs.Button);

    public static InlineKeyboardButton LeaveSession =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_LEAVE_SESSION_BTN",
            SessionLeaveGs.Button);

    public static InlineKeyboardButton RatingFinalShort(int ratingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_RATING_STAGE_SHORT_BTN",
            $"{RatingFinalShortGs.Button}.{ratingPageNumber}");
    }

    public static InlineKeyboardButton RatingFinalShortFromFull(int ratingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_RATING_STAGE_SHORT_FROM_FULL_BTN",
            $"{RatingFinalShortGs.Button}.{ratingPageNumber}");
    }

    public static InlineKeyboardButton RatingFinalFull(int ratingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_RATING_STAGE_FULL_BTN",
            $"{RatingFinalFullGs.Button}.{ratingPageNumber}");
    }

    public static InlineKeyboardButton CreateTeam(int teamPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_SERVER_TEAM_CREATE_BTN",
            $"{CreateTeamGs.Button}.{teamPageNumber}");
    }

    public static InlineKeyboardButton SelectTeam(
        int teamId,
        string teamName)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_GAME_SERVER_TEAM_SELECT_BTN",
                teamName),
            $"{TeamJoinGs.Button}.{teamId}");
    }
}