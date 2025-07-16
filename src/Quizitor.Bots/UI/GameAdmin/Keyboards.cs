using Quizitor.Bots.Behaviors.GameAdmin.Rating;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.GameAdmin;

internal static class Keyboards
{
    public static InlineKeyboardMarkup MainInline =>
        new([
            [Buttons.RoundList, Buttons.Game],
            [Buttons.RatingStageShort(0)]
        ]);

    public static InlineKeyboardMarkup RoundList(
        IEnumerable<Round> rounds,
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            rounds.Select(round => new[]
                {
                    Buttons.Round(
                        round.Id,
                        round.Number,
                        round.Title,
                        pageNumber)
                })
                .Concat([
                    Shared.Buttons.BuildPaginationInlineButtons(
                        pageNumber,
                        pageCount,
                        number => $"{RoundListGa.Button}.{number}",
                        number => $"{RoundListGa.Button}.{number}")
                ])
                .Concat([[Buttons.Main]]));
    }

    public static InlineKeyboardMarkup RoundView(
        int roundId,
        IEnumerable<Question> questions,
        int questionPageNumber,
        int questionPageCount,
        int roundPageNumber)
    {
        return new InlineKeyboardMarkup(
            questions
                .Select(question => new[]
                {
                    Buttons.Question(
                        question.Id,
                        question.Number,
                        question.Title,
                        roundPageNumber,
                        questionPageNumber)
                })
                .Concat([
                    Shared.Buttons.BuildPaginationInlineButtons(
                        questionPageNumber,
                        questionPageCount,
                        number => $"{RoundViewGa.Button}.{roundId}.{roundPageNumber}.{number}",
                        number => $"{RoundViewGa.Button}.{roundId}.{roundPageNumber}.{number}")
                ])
                .Concat([[Buttons.BackToRoundList(roundPageNumber)]]));
    }

    public static InlineKeyboardMarkup QuestionView(
        int questionId,
        int? timingId,
        int roundId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return new InlineKeyboardMarkup([
            [
                timingId.HasValue
                    ? Buttons.QuestionStop(
                        timingId.Value,
                        roundPageNumber,
                        questionPageNumber)
                    : Buttons.QuestionStart(
                        questionId,
                        roundPageNumber,
                        questionPageNumber)
            ],
            [
                Buttons.BackToRound(
                    roundId,
                    roundPageNumber,
                    questionPageNumber)
            ]
        ]);
    }

    public static InlineKeyboardMarkup ActiveQuestionCallbacks(
        string roundTitle,
        string questionTitle,
        int timingId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.QuestionTime(timingId)],
            [
                Buttons.QuestionStopWithTitle(
                    roundTitle,
                    questionTitle,
                    timingId,
                    roundPageNumber,
                    questionPageNumber)
            ]
        ]);
    }

    public static InlineKeyboardMarkup QuestionStopNotification(
        int? nextQuestionId,
        int roundPageNumber,
        int questionPageNumber)
    {
        return new InlineKeyboardMarkup(
            nextQuestionId.HasValue
                ? [[Buttons.QuestionNext(nextQuestionId.Value, roundPageNumber, questionPageNumber)]]
                : [[Buttons.Main]]);
    }

    public static InlineKeyboardMarkup RatingStageShort(
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup([
            [Buttons.RatingStageFull(pageNumber)],
            Shared.Buttons.BuildPaginationInlineButtons(
                pageNumber,
                pageCount,
                number => $"{RatingStageShortGa.Button}.{number}",
                number => $"{RatingStageShortGa.Button}.{number}"),
            [Buttons.Main]
        ]);
    }

    public static InlineKeyboardMarkup RatingStageFull(
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup([
            [Buttons.RatingStageShortFromFull(pageNumber)],
            Shared.Buttons.BuildPaginationInlineButtons(
                pageNumber,
                pageCount,
                number => $"{RatingStageFullGa.Button}.{number}",
                number => $"{RatingStageFullGa.Button}.{number}"),
            [Buttons.Main]
        ]);
    }

    public static InlineKeyboardMarkup Game(bool syncRating)
    {
        return new InlineKeyboardMarkup([
            [syncRating ? Buttons.SyncRatingDisable : Buttons.SyncRatingEnable],
            [Buttons.LeaveSession],
            [Buttons.Main]
        ]);
    }
}