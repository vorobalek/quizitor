using LPlus;
using Quizitor.Bots.Behaviors.GameAdmin.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.GameServer.Default.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.QuestionRules;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.UI.GameServer;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.GameServer.Default;

using Behavior = IMessageTextBehaviorTrait<IDefaultGameServerContext>;
using Context = IMessageTextContext<IDefaultGameServerContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class DefaultGs(
    IEnumerable<IQuestionRuleApplier> questionRuleAppliers,
    ITelegramBotClientFactory telegramBotClientFactory,
    IDbContextProvider dbContextProvider) :
    GameServerBehavior<IDefaultGameServerContext>(
        dbContextProvider),
    Behavior
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    private readonly IReadOnlyDictionary<Type, IQuestionRuleApplier> _questionRuleAppliersMap = questionRuleAppliers
        .ToDictionary(
            x => x.TargetType,
            x => x);

    public override string[] Permissions => [];

    public override bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return !IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update) &&
               (baseContext.Identity.Prompt == null || baseContext.Identity.Prompt.Type == UserPromptType.GameServerAnswer) &&
               base.ShouldPerformMessageText(baseContext);
    }

    public Task PerformMessageTextAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (context.Base.Answer is not { } answerContext)
        {
            return MainPageGs.ResponseAsync(
                context.Base,
                null,
                cancellationToken);
        }

        return ProcessAnswerAsync(
            context,
            answerContext,
            cancellationToken);
    }

    private async Task ProcessAnswerAsync(
        Context context,
        IAnswer answerContext,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo is { Leader: { Id: var leaderId } leader } &&
            context.Base.Identity.User.Id != leaderId)
        {
            await context
                .Base
                .Client
                .SendMessage(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    string.Format(
                        TR.L + "_GAME_SERVER_ANSWER_IS_ALLOWED_ONLY_FOR_LEADER_TXT",
                        leader.Id,
                        leader.GetFullName().EscapeHtml(),
                        TR.L + "_GAME_SERVER_TEAM_BTN",
                        TR.L + "_GAME_SERVER_TEAM_SET_LEADER_BTN"),
                    ParseMode.Html,
                    cancellationToken: cancellationToken);
            return;
        }

        if (answerContext.HasReachedAttemptsCount)
        {
            await context
                .Base
                .Client
                .SendMessage(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    TR.L + "_GAME_SERVER_TOO_MANY_SENDING_PER_QUESTION_TXT",
                    ParseMode.Html,
                    cancellationToken: cancellationToken);
            return;
        }

        if (answerContext.Options.Length > 0 && answerContext.Choice is null)
        {
            await context
                .Base
                .Client
                .SendMessage(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    string.Format(
                        TR.L + "_GAME_SERVER_SENDING_DOESNT_MATCH_OPTIONS_TXT",
                        string.Join(
                            Environment.NewLine,
                            answerContext.Options.Select(option => string.Format(
                                TR.L + "_GAME_SERVER_OPTION_LIST_ITEM_TXT",
                                option.Text.EscapeHtml())))),
                    ParseMode.Html,
                    replyMarkup: Keyboards.Options(answerContext.Options),
                    cancellationToken: cancellationToken);
            return;
        }

        var baseCost = answerContext.Choice?.Cost ?? 0;
        var submission = new Submission
        {
            User = context.Base.Identity.User,
            Team = context.Base.SessionTeamInfo?.Team,
            Question = answerContext.Question,
            Session = context.Base.Session,
            Text = context.MessageText.Truncate(4096),
            Time = Convert.ToInt32(Math.Floor((answerContext.InitiatedAt - answerContext.QuestionTiming.StartTime).TotalSeconds)),
            Score = baseCost
        };
        await _dbContextProvider.Submissions.AddAsync(
            submission,
            cancellationToken);

        var ruleCostMap = new Dictionary<QuestionRule, int>();
        foreach (var rule in answerContext.Rules)
        {
            if (!_questionRuleAppliersMap.TryGetValue(rule.GetType(), out var ruleApplier)) continue;

            var ruleCost = await ruleApplier.ApplyAsync(rule, submission, cancellationToken) ?? 0;
            if (ruleCost != 0)
            {
                ruleCostMap[rule] = ruleCost;
            }
        }

        if (ruleCostMap.Count != 0)
        {
            submission.Score += ruleCostMap.Values.Sum();
            await _dbContextProvider
                .Submissions
                .UpdateAsync(
                    submission,
                    cancellationToken);
        }

        _dbContextProvider
            .AddPostCommitTask(async () =>
            {
                await ResponseAsync(
                    context,
                    answerContext,
                    string.Format(
                        answerContext.AttemptsCountRemaining <= 0
                            ? TR.L + "_GAME_SERVER_LAST_SENDING_RECEIVED_TXT"
                            : TR.L + "_GAME_SERVER_SENDING_RECEIVED_TXT",
                        submission.Text.EscapeHtml(),
                        answerContext.AttemptsCountRemaining),
                    Keyboards.Options(answerContext.Options),
                    cancellationToken);

                if (ShouldNotifyGameAdmins(
                        answerContext.Question,
                        answerContext.Options,
                        answerContext.Rules,
                        baseCost,
                        ruleCostMap))
                {
                    await NotifyGameAdminsAsync(
                        context.Base.UpdateContext,
                        submission,
                        answerContext.Round,
                        answerContext.Question,
                        baseCost,
                        ruleCostMap,
                        answerContext.AdminBots,
                        answerContext.AdminUsers,
                        cancellationToken);
                }
            });
    }

    private static bool ShouldNotifyGameAdmins(
        Question question,
        ICollection<QuestionOption> options,
        ICollection<QuestionRule> rules,
        int baseCost,
        IReadOnlyDictionary<QuestionRule, int> ruleCostMap)
    {
        return question.SubmissionNotificationType switch
        {
            SubmissionNotificationType.All => true,
            SubmissionNotificationType.AnyScored => baseCost + ruleCostMap.Values.Sum() > 0,
            SubmissionNotificationType.FullScored => 
                baseCost == (options.MaxBy(option => option.Cost)?.Cost ?? 0) &&
                ruleCostMap.Values.Sum() == rules.Sum(rule => rule.Cost),
            SubmissionNotificationType.WrongOnly => baseCost == 0,
            _ => false
        };
    }

    private async Task NotifyGameAdminsAsync(
        UpdateContext updateContext,
        Submission submission,
        Round round,
        Question question,
        int baseCost,
        IReadOnlyDictionary<QuestionRule, int> ruleCostMap,
        ICollection<Bot> adminBots,
        ICollection<User> adminUsers,
        CancellationToken cancellationToken)
    {
        var text = string.Format(
            TR.L + "_GAME_ADMIN_SUBMISSION_RECEIVED_WITH_RULES_TXT",
            submission.Score == 0
                ? TR.L + "_GAME_ADMIN_SENDING_ACCEPTED_TXT"
                : string.Format(
                    TR.L + "_GAME_ADMIN_SENDING_ACCEPTED_WITH_SCORE_TXT",
                    submission.Score),
            submission.Team is { } team
                ? string.Format(
                    TR.L + "_GAME_ADMIN_SUBMISSION_RECEIVED_FROM_TEAM_TXT",
                    team.Name.EscapeHtml())
                : string.Format(
                    TR.L + "_GAME_ADMIN_SUBMISSION_RECEIVED_FROM_USER_TXT",
                    submission.User.Id,
                    submission.User.GetFullName().EscapeHtml()),
            round.Title.EscapeHtml(),
            question.Title.EscapeHtml(),
            submission.Text.EscapeHtml(),
            baseCost,
            ruleCostMap.Count > 0
                ? string.Join(
                    string.Empty,
                    ruleCostMap.Select(keyValue => string.Format(
                        TR.L + "_GAME_ADMIN_QUESTION_RULE_LIST_ITEM_TXT",
                        string.Format(
                            TR.L + $"_GAME_ADMIN_QUESTION_RULE_{keyValue.Key.GetType().Name}",
                            keyValue.Key.GetGameAdminLocalizationArgs()),
                        keyValue.Value)))
                : TR.L + "_SHARED_NO_TXT",
            submission.Time);

        var adminBotClients = adminBots
            .ToDictionary(
                x => x.Id,
                telegramBotClientFactory.CreateForBot);

        foreach (var adminUser in adminUsers)
        {
            if (!adminUser.GameAdminId.HasValue ||
                !adminBotClients.TryGetValue(adminUser.GameAdminId.Value, out var client)) continue;

            await client
                .SendMessage(
                    updateContext,
                    adminUser.Id,
                    text,
                    ParseMode.Html,
                    cancellationToken: cancellationToken);
        }
    }

    private async Task ResponseAsync(
        Context context,
        IAnswer answerContext,
        string text,
        ReplyMarkup? replyMarkup,
        CancellationToken cancellationToken)
    {
        if (context.Base.SessionTeamInfo is { } teamInfo)
        {
            var gameBotClients = answerContext.GameBots
                .ToDictionary(
                    x => x.Id,
                    telegramBotClientFactory.CreateForBot);

            foreach (var member in teamInfo.Members)
            {
                if (!member.GameServerId.HasValue ||
                    !gameBotClients.TryGetValue(member.GameServerId.Value, out var client)) continue;

                await client
                    .SendMessage(
                        context.Base.UpdateContext,
                        member.Id,
                        text,
                        ParseMode.Html,
                        replyMarkup: replyMarkup,
                        cancellationToken: cancellationToken);
            }

            return;
        }

        await context
            .Base
            .Client
            .SendMessage(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                text,
                ParseMode.Html,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
    }

    protected override async Task<IDefaultGameServerContext?> PrepareGameServerInternalAsync(
        IGameServerContext gameServerContext,
        CancellationToken cancellationToken)
    {
        var messageTextContext = IMessageTextContext.Create(gameServerContext);
        if (messageTextContext is null) return null;

        if (await _dbContextProvider
                .QuestionTimings
                .GetActiveBySessionIdOrDefaultAsync(
                    gameServerContext.Session.Id,
                    cancellationToken) is not { } questionTiming ||
            gameServerContext.Identity.Prompt?.Type is not UserPromptType.GameServerAnswer ||
            gameServerContext.Identity.Prompt.Subject is not { } promptSubjectString ||
            JsonSerializerHelper.TryDeserialize<GameServerAnswerPromptSubject>(promptSubjectString) is not { } promptSubject ||
            promptSubject.TimingId != questionTiming.Id)
        {
            return IDefaultGameServerContext.Create(
                null,
                gameServerContext);
        }

        var question = await _dbContextProvider
            .Questions
            .GetByIdAsync(
                questionTiming.QuestionId,
                cancellationToken);

        var round = await _dbContextProvider
            .Rounds
            .GetByIdAsync(
                question.RoundId,
                cancellationToken);

        var options = await _dbContextProvider
            .Questions
            .GetOptionsByQuestionIdAsync(
                question.Id,
                cancellationToken);

        var choice = options.FirstOrDefault(x =>
            string.Equals(
                x.Text,
                messageTextContext.MessageText,
                StringComparison.OrdinalIgnoreCase));

        var rules = await _dbContextProvider
            .Questions
            .GetRulesByQuestionIdAsync(
                question.Id,
                cancellationToken);

        var submissionsCount = gameServerContext.SessionTeamInfo is { } teamInfo
            ? await _dbContextProvider
                .Submissions
                .CountForTeamIdByQuestionIdAndSessionIdAsync(
                    teamInfo.Team.Id,
                    question.Id,
                    gameServerContext.Session.Id,
                    cancellationToken)
            : await _dbContextProvider
                .Submissions
                .CountForUserIdWithoutTeamByQuestionIdAndSessionIdAsync(
                    gameServerContext.Identity.User.Id,
                    question.Id,
                    gameServerContext.Session.Id,
                    cancellationToken);

        var attemptsCountExceeded = submissionsCount >= question.Attempts;
        var attemptsCountRemaining = question.Attempts - submissionsCount - 1;

        var gameBots = await _dbContextProvider
            .Bots
            .GetActiveGameServersAsync(cancellationToken);

        var adminBots = await _dbContextProvider
            .Bots
            .GetActiveGameAdminsAsync(cancellationToken);

        var adminUsers = await _dbContextProvider
            .Users
            .GetGameAdminsBySessionIdAsync(
                gameServerContext.Session.Id,
                TelegramBotConfiguration.AuthorizedUserIds,
                cancellationToken);

        var initiatedAt = await _dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken);

        return IDefaultGameServerContext.Create(
            IAnswer.Create(
                round,
                question,
                questionTiming,
                options,
                choice,
                rules,
                attemptsCountExceeded,
                attemptsCountRemaining,
                gameBots,
                adminBots,
                adminUsers,
                initiatedAt),
            gameServerContext);
    }
}