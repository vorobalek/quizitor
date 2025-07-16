using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Quizitor.Data.Extensions;
using Quizitor.Data.Repositories;

namespace Quizitor.Data;

internal sealed class DbContextProvider(
    ApplicationDbContext dbContext,
    IBotRepository botRepository,
    IBotCommandRepository botCommandRepository,
    IBotInteractionRepository botInteractionRepository,
    IGameRepository gameRepository,
    IQuestionRepository questionRepository,
    IQuestionTimingRepository questionTimingRepository,
    IQuestionNotifyTimingRepository questionNotifyTimingRepository,
    IQuestionStopTimingRepository questionStopTimingRepository,
    IRoleRepository roleRepository,
    IRoundRepository roundRepository,
    ISessionRepository sessionRepository,
    ISubmissionRepository submissionRepository,
    IUserRepository userRepository,
    ITeamRepository teamRepository,
    IMailingRepository mailingRepository) : IDbContextProvider
{
    private readonly List<Func<Task>> _postCommitTasks = [];

    public void AddPostCommitTask(Func<Task> task)
    {
        _postCommitTasks.Add(task);
    }

    public void ClearPostCommitTasks()
    {
        _postCommitTasks.Clear();
    }

    public IReadOnlyCollection<Func<Task>> PostCommitTasks => _postCommitTasks;

    public Task<DateTimeOffset> GetServerDateTimeOffsetAsync(CancellationToken cancellationToken)
    {
        return dbContext.GetServerDateTimeOffsetAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken)
    {
        return dbContext.Database.BeginTransactionAsync(
            isolationLevel,
            cancellationToken);
    }

    public IBotRepository Bots => botRepository;
    public IBotCommandRepository BotCommands => botCommandRepository;
    public IBotInteractionRepository BotInteractions => botInteractionRepository;
    public IGameRepository Games => gameRepository;
    public IRoundRepository Rounds => roundRepository;
    public IQuestionRepository Questions => questionRepository;
    public IQuestionTimingRepository QuestionTimings => questionTimingRepository;
    public IQuestionNotifyTimingRepository QuestionNotifyTimings => questionNotifyTimingRepository;
    public IQuestionStopTimingRepository QuestionStopTimings => questionStopTimingRepository;
    public ISessionRepository Sessions => sessionRepository;
    public ISubmissionRepository Submissions => submissionRepository;
    public IRoleRepository Roles => roleRepository;
    public IUserRepository Users => userRepository;
    public ITeamRepository Teams => teamRepository;
    public IMailingRepository Mailings => mailingRepository;
}