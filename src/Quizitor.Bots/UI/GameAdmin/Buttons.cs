using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin;
using Quizitor.Bots.Behaviors.GameAdmin.Games;
using Quizitor.Bots.Behaviors.GameAdmin.Rating;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions;
using Quizitor.Bots.Behaviors.GameAdmin.Sessions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.GameAdmin;

internal static class Buttons
{
    public static InlineKeyboardButton Main =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_MAIN_BTN",
            MainPageGa.Button);

    public static InlineKeyboardButton RoundList =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_ROUNDS_BTN",
            $"{RoundListGa.Button}.0");

    public static InlineKeyboardButton Game =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_GAME_BTN",
            GameGa.Button);

    public static InlineKeyboardButton SyncRatingEnable =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_SYNC_RATING_ENABLE_BTN",
            SyncRatingEnableGameGa.Button);

    public static InlineKeyboardButton SyncRatingDisable =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_SYNC_RATING_DISABLE_BTN",
            SyncRatingDisableGameGa.Button);

    public static InlineKeyboardButton LeaveSession =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_LEAVE_SESSION_BTN",
            SessionLeaveGa.Button);

    public static InlineKeyboardButton RatingStageShort(int ratingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_RATING_STAGE_SHORT_BTN",
            $"{RatingStageShortGa.Button}.{ratingPageNumber}");
    }

    public static InlineKeyboardButton RatingStageShortFromFull(int ratingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_RATING_STAGE_SHORT_FROM_FULL_BTN",
            $"{RatingStageShortGa.Button}.{ratingPageNumber}");
    }

    public static InlineKeyboardButton RatingStageFull(int ratingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_RATING_STAGE_FULL_BTN",
            $"{RatingStageFullGa.Button}.{ratingPageNumber}");
    }

    public static InlineKeyboardButton Round(
        int roundId,
        int number,
        string title,
        int roundPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_GAME_ADMIN_ROUND_VIEW_BTN",
                number,
                title),
            $"{RoundViewGa.Button}.{roundId}.{roundPageNumber}.0");
    }

    public static InlineKeyboardButton Question(
        int questionId,
        int number,
        string title,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_GAME_ADMIN_QUESTION_VIEW_BTN",
                number,
                title),
            $"{QuestionViewGa.Button}.{questionId}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton BackToRoundList(
        int roundPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{RoundListGa.Button}.{roundPageNumber}");
    }

    public static InlineKeyboardButton QuestionStart(
        int questionId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_START_BTN",
            $"{QuestionStartGa.Button}.{questionId}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton QuestionStop(
        int timingId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_STOP_BTN",
            $"{QuestionStopGa.Button}.{timingId}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton QuestionStopWithTitle(
        string roundTitle,
        string questionTitle,
        int timingId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_GAME_ADMIN_STOP_WITH_TITLE_BTN",
                roundTitle,
                questionTitle),
            $"{QuestionStopGa.Button}.{timingId}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton BackToRound(
        int roundId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{RoundViewGa.Button}.{roundId}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton QuestionTime(
        int timingId)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_TIME_BTN",
            $"{QuestionTimeGa.Button}.{timingId}");
    }

    public static InlineKeyboardButton QuestionNext(
        int questionId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_GAME_ADMIN_NEXT_QUESTION_BTN",
            $"{QuestionViewGa.Button}.{questionId}.{roundPageNumber}.{questionPageNumber}");
    }
}