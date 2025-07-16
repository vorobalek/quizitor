using System.Text;
using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rating.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Helpers;
using Quizitor.Bots.UI.GameAdmin;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Redis.Contracts;
using Quizitor.Redis.Storage.Rating;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameAdmin.Rating;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRatingStageFullGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IRatingStageFullGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RatingStageFullGa(
    IRatingFullStageRedisStorage ratingFullStageRedisStorage,
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IRatingStageFullGameAdminContext>(
        dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>ratingstagefull</b>.{ratingPageNumber}
    /// </summary>
    public const string Button = "ratingstagefull";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string[] GameAdminPermissions => [UserPermission.GameAdminRatingStageShortView];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                GetText(
                    context,
                    out var actualPageNumber,
                    out var actualPageCount),
                ParseMode.Html,
                replyMarkup: Keyboards.RatingStageFull(
                    actualPageNumber,
                    actualPageCount),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IRatingStageFullGameAdminContext?> PrepareGameAdminInternalAsync(
        IGameAdminContext gameAdminContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameAdminContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } ratingPageNumberString
            ] &&
            int.TryParse(ratingPageNumberString, out var ratingPageNumber))
        {
            var ratingFull = await ratingFullStageRedisStorage
                .ReadAsync(
                    gameAdminContext.Session.Id,
                    cancellationToken);

            var lines = (ratingFull?.Lines ?? [])
                .OrderByDescending(x => x.ScorePerRound.Sum(e => e.Value.Sum(v => v.Value)))
                .ThenBy(x => x.TimePerRound.Sum(e => e.Value.Sum(v => v.Value)))
                .ToArray();

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            TimeSpan? timeSinceLastUpdate = ratingFull?.LastUpdatedAt is not null
                ? serverTime - ratingFull.LastUpdatedAt
                : null;

            return IRatingStageFullGameAdminContext.Create(
                lines,
                ratingPageNumber,
                timeSinceLastUpdate,
                gameAdminContext);
        }

        return null;
    }

    private static string GetText(
        Context context,
        out int actualPageNumber,
        out int actualPageCount)
    {
        actualPageNumber = 0;
        actualPageCount = 0;
        if (context.Base.Lines is not { Length: > 0 })
            return TR.L + "_GAME_ADMIN_RATING_NODATA_TXT";

        var lines = context.Base.Lines
            .OrderByDescending(x => x.ScorePerRound.Sum(e => e.Value.Sum(v => v.Value)))
            .ThenBy(x => x.TimePerRound.Sum(e => e.Value.Sum(v => v.Value)))
            .ToArray();

        // Pre-compute positions, scores, and times
        var positionsDict = new Dictionary<RatingFullLineDto, (int Place, int Score, int Time)>();
        var place = 0;
        var prevScore = int.MaxValue;
        var prevTime = int.MaxValue;

        foreach (var line in lines)
        {
            var totalScore = line.ScorePerRound.Sum(e => e.Value.Sum(v => v.Value));
            var totalTime = line.TimePerRound.Sum(e => e.Value.Sum(v => v.Value));

            if (totalScore < prevScore ||
                totalScore == prevScore && totalTime > prevTime)
            {
                place++;
            }

            positionsDict[line] = (place, totalScore, totalTime);
            prevScore = totalScore;
            prevTime = totalTime;
        }

        return lines.GetPage(
            GetLineFunc,
            GetPageFunc,
            context.Base.RatingPageNumber,
            out actualPageNumber,
            out actualPageCount);

        string GetPageFunc(ICollection<string> pageLines)
        {
            return string
                .Format(
                    TR.L + "_GAME_ADMIN_RATING_FULL_TXT",
                    string.Join(Environment.NewLine, pageLines),
                    context.Base.TimeSinceLastUpdate?.Days ?? 0,
                    context.Base.TimeSinceLastUpdate?.Hours ?? 0,
                    context.Base.TimeSinceLastUpdate?.Minutes ?? 0,
                    context.Base.TimeSinceLastUpdate?.Seconds ?? 0);
        }

        string GetLineFunc(RatingFullLineDto line, out bool isSpecial)
        {
            var (finalPlace, totalScore, totalTime) = positionsDict[line];

            var name = TR.L + "_GAME_ADMIN_RATING_LINE_UNKNOWN_TXT";
            if (line.UserId.HasValue && !string.IsNullOrWhiteSpace(line.UserFullName))
                name = string.Format(
                    TR.L + "_GAME_ADMIN_RATING_LINE_USER_TXT",
                    line.UserId.Value,
                    line.UserFullName);
            else if (line.TeamId.HasValue && !string.IsNullOrWhiteSpace(line.TeamName))
                name = string.Format(
                    TR.L + "_GAME_ADMIN_RATING_LINE_TEAM_TXT",
                    line.TeamName);

            var minRoundNumber = context.Base.Lines.Min(x => x.ScorePerRound.Min(e => e.Key));
            var maxRoundNumber = context.Base.Lines.Max(x => x.ScorePerRound.Max(e => e.Key));

            var fullLineBuilder = new StringBuilder();
            for (var roundNumber = minRoundNumber; roundNumber <= maxRoundNumber; ++roundNumber)
            {
                var roundScore = line.ScorePerRound.TryGetValue(roundNumber, out var roundScoreLines)
                    ? roundScoreLines.Sum(x => x.Value)
                    : 0;
                var roundTime = line.ScorePerRound.TryGetValue(roundNumber, out var roundTimeLines)
                    ? roundTimeLines.Sum(x => x.Value)
                    : 0;

                fullLineBuilder.Append(string
                    .Format(
                        TR.L + "_GAME_ADMIN_RATING_FULL_ROUND_ITEM_TXT",
                        roundNumber,
                        roundScore,
                        roundTime));

                if (roundNumber < maxRoundNumber)
                {
                    fullLineBuilder.Append(TR.L + "_GAME_ADMIN_RATING_FULL_ROUND_ITEM_SEPARATOR_TXT");
                }
            }

            var lineText = string.Format(
                TR.L + "_GAME_ADMIN_RATING_FULL_LINE_TXT",
                finalPlace,
                name,
                totalScore,
                fullLineBuilder,
                totalTime);

            isSpecial = false;

            return lineText;
        }
    }
}