using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Quizitor.Data.Repositories;

namespace Quizitor.Data;

public interface IDbContextProvider
{
    IReadOnlyCollection<Func<Task>> PostCommitTasks { get; }

    IBotRepository Bots { get; }
    IBotCommandRepository BotCommands { get; }
    IBotInteractionRepository BotInteractions { get; }
    IGameRepository Games { get; }
    IRoundRepository Rounds { get; }
    IQuestionRepository Questions { get; }
    IQuestionTimingRepository QuestionTimings { get; }
    IQuestionNotifyTimingRepository QuestionNotifyTimings { get; }
    IQuestionStopTimingRepository QuestionStopTimings { get; }
    ISessionRepository Sessions { get; }
    ISubmissionRepository Submissions { get; }
    IRoleRepository Roles { get; }
    IUserRepository Users { get; }
    ITeamRepository Teams { get; }
    IMailingRepository Mailings { get; }
    void AddPostCommitTask(Func<Task> task);
    void ClearPostCommitTasks();

    Task<DateTimeOffset> GetServerDateTimeOffsetAsync(CancellationToken cancellationToken);

    Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken);
}