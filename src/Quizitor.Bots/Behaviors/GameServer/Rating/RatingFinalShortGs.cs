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

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRatingFinalShortGameServerContext>;
using Context = ICallbackQueryDataPrefixContext<IRatingFinalShortGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RatingFinalShortGs(
    IRatingShortFinalRedisStorage ratingShortFinalRedisStorage,
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<IRatingFinalShortGameServerContext>(
        dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>ratingfinalshort</b>.{ratingPageNumber}
    /// </summary>
    public const string Button = "ratingfinalshort";

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
                replyMarkup: Keyboards.RatingFinalShort(
                    actualPageNumber,
                    actualPageCount),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IRatingFinalShortGameServerContext?> PrepareGameServerInternalAsync(
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
            var ratingShort = await ratingShortFinalRedisStorage
                .ReadAsync(
                    gameServerContext.Session.Id,
                    cancellationToken);

            var lines = (ratingShort?.Lines ?? [])
                .OrderByDescending(x => x.TotalScore)
                .ThenBy(x => x.TotalTime)
                .ToArray();

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            TimeSpan? timeSinceLastUpdate = ratingShort?.LastUpdatedAt is not null
                ? serverTime - ratingShort.LastUpdatedAt
                : null;

            return IRatingFinalShortGameServerContext.Create(
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
            .OrderByDescending(x => x.TotalScore)
            .ThenBy(x => x.TotalTime)
            .ToArray();

        // Pre-compute positions, scores, and times
        var positionsDict = new Dictionary<RatingShortLineDto, (int Place, int Score, int Time)>();
        var place = 0;
        var prevScore = int.MaxValue;
        var prevTime = int.MaxValue;

        foreach (var line in lines)
        {
            if (line.TotalScore < prevScore ||
                line.TotalScore == prevScore && line.TotalTime > prevTime)
            {
                place++;
            }

            positionsDict[line] = (place, line.TotalScore, line.TotalTime);
            prevScore = line.TotalScore;
            prevTime = line.TotalTime;
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
                        ? TR.L + "_GAME_SERVER_RATING_SHORT_TXT"
                        : TR.L + "_GAME_SERVER_RATING_SHORT_SYNC_DISABLED_TXT",
                    string.Join(Environment.NewLine, pageLines),
                    context.Base.TimeSinceLastUpdate?.Days ?? 0,
                    context.Base.TimeSinceLastUpdate?.Hours ?? 0,
                    context.Base.TimeSinceLastUpdate?.Minutes ?? 0,
                    context.Base.TimeSinceLastUpdate?.Seconds ?? 0);
        }

        string GetLineFunc(RatingShortLineDto line, out bool isSpecial)
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

            var lineText = string.Format(
                TR.L + "_GAME_SERVER_RATING_SHORT_LINE_TXT",
                finalPlace,
                name,
                totalScore,
                totalTime);

            isSpecial = line.UserId.HasValue && context.Base.Identity.User.Id == line.UserId.Value ||
                        line.TeamId.HasValue && context.Base.SessionTeamInfo?.Team.Id == line.TeamId.Value;

            return lineText;
        }
    }
}