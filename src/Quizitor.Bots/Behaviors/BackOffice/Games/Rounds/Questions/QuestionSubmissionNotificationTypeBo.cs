using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;
using Quizitor.Data.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions;

using Context = ICallbackQueryDataPrefixContext<IQuestionBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class QuestionSubmissionNotificationTypeBo(
    IDbContextProvider dbContextProvider) :
    QuestionViewBo(dbContextProvider)
{
    /// <summary>
    ///     <b>question</b>.{questionId}.{gamePageNumber}.{roundPageNumber}.{questionPageNumber}
    /// </summary>
    public new const string Button = "questionsubmissionnotificationtype";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        context.Base.Question.SubmissionNotificationType = (SubmissionNotificationType)(
            (Convert.ToInt32(context.Base.Question.SubmissionNotificationType) + 1) % Enum.GetValues<SubmissionNotificationType>().Length);

        await _dbContextProvider
            .Questions
            .UpdateAsync(
                context.Base.Question,
                cancellationToken);

        _dbContextProvider
            .AddPostCommitTask(async () =>
                await ResponseAsync(context, cancellationToken));
    }
}