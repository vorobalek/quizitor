using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;

internal interface IDeleteGameBackOfficeContext : IBackOfficeContext
{
    Game Game { get; }
    int GamePageNumber { get; }
    int RoundsCount { get; }
    int QuestionsCount { get; }
    int OptionsCount { get; }
    int RulesCount { get; }
    int SessionsCount { get; }
    int SubmissionsCount { get; }
    int QuestionTimingsCount { get; }
    int TimingNotifyEventsCount { get; }
    int TimingStopEventsCount { get; }
    int ConnectedUsersCount { get; }

    static IDeleteGameBackOfficeContext Create(
        Game game,
        int gamePageNumber,
        int roundsCount,
        int questionsCount,
        int optionsCount,
        int rulesCount,
        int sessionsCount,
        int submissionsCount,
        int questionTimingsCount,
        int timingNotifyEventsCount,
        int timingStopEventsCount,
        int connectedUsersCount,
        IBackOfficeContext baseContext)
    {
        return new DeleteGameBackOfficeContext(
            game,
            gamePageNumber,
            roundsCount,
            questionsCount,
            optionsCount,
            rulesCount,
            sessionsCount,
            submissionsCount,
            questionTimingsCount,
            timingNotifyEventsCount,
            timingStopEventsCount,
            connectedUsersCount,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record DeleteGameBackOfficeContext(
        Game Game,
        int GamePageNumber,
        int RoundsCount,
        int QuestionsCount,
        int OptionsCount,
        int RulesCount,
        int SessionsCount,
        int SubmissionsCount,
        int QuestionTimingsCount,
        int TimingNotifyEventsCount,
        int TimingStopEventsCount,
        int ConnectedUsersCount,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IDeleteGameBackOfficeContext;
}