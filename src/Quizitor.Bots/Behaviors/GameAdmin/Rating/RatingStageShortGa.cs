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

using Behavior = ICallbackQueryDataPrefixBehaviorTrait<IRatingStageShortGameAdminContext>;
using Context = ICallbackQueryDataPrefixContext<IRatingStageShortGameAdminContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class RatingStageShortGa(
    IRatingShortStageRedisStorage ratingShortStageRedisStorage,
    IDbContextProvider dbContextProvider) :
    GameAdminBehavior<IRatingStageShortGameAdminContext>(
        dbContextProvider),
    Behavior
{
    /// <summary>
    ///     <b>ratingstageshort</b>.{ratingPageNumber}
    /// </summary>
    public const string Button = "ratingstageshort";

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
                replyMarkup: Keyboards.RatingStageShort(
                    actualPageNumber,
                    actualPageCount),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IRatingStageShortGameAdminContext?> PrepareGameAdminInternalAsync(
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
            var ratingShort = await ratingShortStageRedisStorage
                .ReadAsync(
                    gameAdminContext.Session.Id,
                    cancellationToken);

            var lines = (ratingShort?.Lines ?? [])
                .OrderByDescending(x => x.TotalScore)
                .ThenBy(x => x.TotalTime)
                .ToArray();

            var serverTime = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);
            TimeSpan? timeSinceLastUpdate = ratingShort?.LastUpdatedAt is not null
                ? serverTime - ratingShort.LastUpdatedAt
                : null;

            return IRatingStageShortGameAdminContext.Create(
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
                    TR.L + "_GAME_ADMIN_RATING_SHORT_TXT",
                    string.Join(Environment.NewLine, pageLines),
                    context.Base.TimeSinceLastUpdate?.Days ?? 0,
                    context.Base.TimeSinceLastUpdate?.Hours ?? 0,
                    context.Base.TimeSinceLastUpdate?.Minutes ?? 0,
                    context.Base.TimeSinceLastUpdate?.Seconds ?? 0);
        }

        string GetLineFunc(RatingShortLineDto line, out bool isSpecial)
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

            var lineText = string.Format(
                TR.L + "_GAME_ADMIN_RATING_SHORT_LINE_TXT",
                finalPlace,
                name,
                totalScore,
                totalTime);

            isSpecial = false;

            return lineText;
        }
    }
}