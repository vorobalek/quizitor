using Quizitor.Bots.Behaviors.GameServer.Rating;
using Quizitor.Bots.Behaviors.GameServer.Teams;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.GameServer;

internal static class Keyboards
{
    public static readonly ReplyKeyboardRemove Remove = new();

    public static InlineKeyboardMarkup NoTeam => new([
        [Buttons.TeamList],
        [Buttons.Main]
    ]);

    public static ReplyMarkup Options(IEnumerable<QuestionOption> options)
    {
        var optionsArray = options as QuestionOption[] ?? [.. options];
        return optionsArray.Length != 0
            ? new ReplyKeyboardMarkup(
                optionsArray.Select<QuestionOption, IEnumerable<KeyboardButton>>(x =>
                    [new KeyboardButton(x.Text)]))
            : Remove;
    }

    public static InlineKeyboardMarkup MainPage(
        bool hasTeam)
    {
        return new InlineKeyboardMarkup([
            [hasTeam ? Buttons.Team : Buttons.TeamList, Buttons.Game],
            [Buttons.RatingFinalShort(0)]
        ]);
    }

    public static InlineKeyboardMarkup Team(
        bool setLeader)
    {
        return new InlineKeyboardMarkup([
            [Buttons.TeamQr],
            [setLeader ? Buttons.TeamSetLeader : Buttons.TeamUnsetLeader, Buttons.TeamLeave],
            [Buttons.Main]
        ]);
    }

    public static InlineKeyboardMarkup Game()
    {
        return new InlineKeyboardMarkup([
            [Buttons.LeaveSession],
            [Buttons.Main]
        ]);
    }

    public static InlineKeyboardMarkup RatingFinalShort(
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup([
            [Buttons.RatingFinalFull(pageNumber)],
            Shared.Buttons.BuildPaginationInlineButtons(
                pageNumber,
                pageCount,
                number => $"{RatingFinalShortGs.Button}.{number}",
                number => $"{RatingFinalShortGs.Button}.{number}"),
            [Buttons.Main]
        ]);
    }

    public static InlineKeyboardMarkup RatingFinalFull(
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup([
            [Buttons.RatingFinalShortFromFull(pageNumber)],
            Shared.Buttons.BuildPaginationInlineButtons(
                pageNumber,
                pageCount,
                number => $"{RatingFinalFullGs.Button}.{number}",
                number => $"{RatingFinalFullGs.Button}.{number}"),
            [Buttons.Main]
        ]);
    }

    public static InlineKeyboardMarkup TeamList(
        Team[] teams,
        int teamPageNumber,
        int teamPageCount)
    {
        return new InlineKeyboardMarkup(
            new[]
                {
                    new[]
                    {
                        Buttons.CreateTeam(teamPageNumber)
                    }
                }
                .Concat(
                    teams.Select(team => new[]
                        {
                            Buttons.SelectTeam(
                                team.Id,
                                team.Name)
                        })
                        .Concat([
                            Shared.Buttons.BuildPaginationInlineButtons(
                                teamPageNumber,
                                teamPageCount,
                                number => $"{TeamListGs.Button}.{number}",
                                number => $"{TeamListGs.Button}.{number}")
                        ])
                        .Concat([[Buttons.Main]])));
    }
}