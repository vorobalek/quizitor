using System.Text;
using LPlus;
using Quizitor.Bots.Behaviors.GameServer.Rating.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Helpers;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Data;
using Quizitor.Redis.Contracts;
using Quizitor.Redis.Storage.Rating;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.GameServer.Rating;

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRatingFinalFullGameServerContext>;
using Context = ICallbackQueryDataPrefixContext<IRatingFinalFullGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RatingFinalFullGs(
    IRatingFullFinalRedisStorage ratingFullFinalRedisStorage,
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<IRatingFinalFullGameServerContext>(
        dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>ratingfinalfull</b>.{ratingPageNumber}
    /// </summary>
    public const string Button = "ratingfinalfull";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions => [];

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
                replyMarkup: Keyboards.RatingFinalFull(
                    actualPageNumber,
                    actualPageCount),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IRatingFinalFullGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(gameServerContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } ratingPageNumberString
            ] &&
            int.TryParse(ratingPageNumberString, out var ratingPageNumber))
        {
            var ratingFull = await ratingFullFinalRedisStorage
                .ReadAsync(
                    gameServerContext.Session.Id,
                    cancellationToken);

            var lines = (ratingFull?.Lines ?? [])
                .OrderByDescending(x => x.ScorePerRound.Sum(e => e.Value.Sum(v => v.Value)))
                .ThenBy(x => x.TimePerRound.Sum(e => e.Value.Sum(v => v.Value)))
                .ToArray();

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            TimeSpan? timeSinceLastUpdate = ratingFull?.LastUpdatedAt is not null
                ? serverTime - ratingFull.LastUpdatedAt
                : null;

            return IRatingFinalFullGameServerContext.Create(
                lines,
                ratingPageNumber,
                timeSinceLastUpdate,
                gameServerContext);
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
            return TR.L + "_GAME_SERVER_RATING_NODATA_TXT";

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
                    context.Base.Session.SyncRating
                        ? TR.L + "_GAME_SERVER_RATING_FULL_TXT"
                        : TR.L + "_GAME_SERVER_RATING_FULL_SYNC_DISABLED_TXT",
                    string.Join(Environment.NewLine, pageLines),
                    context.Base.TimeSinceLastUpdate?.Days ?? 0,
                    context.Base.TimeSinceLastUpdate?.Hours ?? 0,
                    context.Base.TimeSinceLastUpdate?.Minutes ?? 0,
                    context.Base.TimeSinceLastUpdate?.Seconds ?? 0);
        }

        string GetLineFunc(RatingFullLineDto line, out bool isSpecial)
        {
            var (finalPlace, totalScore, totalTime) = positionsDict[line];

            var name = TR.L + "_GAME_SERVER_RATING_LINE_UNKNOWN_TXT";
            if (line.UserId.HasValue && !string.IsNullOrWhiteSpace(line.UserFullName))
            {
                var isSelfLine = context.Base.Identity.User.Id == line.UserId.Value;
                name = string.Format(
                    isSelfLine
                        ? TR.L + "_GAME_SERVER_RATING_LINE_USER_SELF_TXT"
                        : TR.L + "_GAME_SERVER_RATING_LINE_USER_TXT",
                    line.UserId.Value,
                    line.UserFullName);
            }
            else if (line.TeamId.HasValue && !string.IsNullOrWhiteSpace(line.TeamName))
            {
                var isSelfLine = context.Base.SessionTeamInfo?.Team.Id == line.TeamId.Value;
                name = string.Format(
                    isSelfLine
                        ? TR.L + "_GAME_SERVER_RATING_LINE_TEAM_SELF_TXT"
                        : TR.L + "_GAME_SERVER_RATING_LINE_TEAM_TXT",
                    line.TeamName);
            }

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
                        TR.L + "_GAME_SERVER_RATING_FULL_ROUND_ITEM_TXT",
                        roundNumber,
                        roundScore,
                        roundTime));

                if (roundNumber < maxRoundNumber)
                {
                    fullLineBuilder.Append(TR.L + "_GAME_SERVER_RATING_FULL_ROUND_ITEM_SEPARATOR_TXT");
                }
            }

            var lineText = string.Format(
                TR.L + "_GAME_SERVER_RATING_FULL_LINE_TXT",
                finalPlace,
                name,
                totalScore,
                fullLineBuilder,
                totalTime);

            isSpecial = line.UserId.HasValue && context.Base.Identity.User.Id == line.UserId.Value ||
                        line.TeamId.HasValue && context.Base.SessionTeamInfo?.Team.Id == line.TeamId.Value;

            return lineText;
        }
    }
}