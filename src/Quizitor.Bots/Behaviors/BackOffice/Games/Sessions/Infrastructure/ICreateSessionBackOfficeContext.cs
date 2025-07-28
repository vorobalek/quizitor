using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Sessions.Infrastructure;

internal interface ICreateSessionBackOfficeContext : IBackOfficeContext
{
    IWrongPrompt? WrongPrompt { get; }
    INewPrompt? NewPrompt { get; }
    IDataError? DataError { get; }
    INewSession? NewSession { get; }

    static ICreateSessionBackOfficeContext Create(
        IWrongPrompt? wrongPrompt,
        INewPrompt? newPrompt,
        IDataError? dataError,
        INewSession? newSession,
        IBackOfficeContext baseContext)
    {
        return new CreateSessionBackOfficeContext(
            wrongPrompt,
            newPrompt,
            dataError,
            newSession,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record CreateSessionBackOfficeContext(
        IWrongPrompt? WrongPrompt,
        INewPrompt? NewPrompt,
        IDataError? DataError,
        INewSession? NewSession,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ICreateSessionBackOfficeContext;

    internal interface IWrongPrompt
    {
        UserPromptType PromptType { get; }

        static IWrongPrompt Create(UserPromptType promptType)
        {
            return new WrongPrompt(promptType);
        }

        private record WrongPrompt(UserPromptType PromptType) : IWrongPrompt;
    }

    internal interface INewPrompt
    {
        Game Game { get; }
        int GamePageNumber { get; }
        int SessionPageNumber { get; }

        static INewPrompt Create(
            Game game,
            int gamePageNumber,
            int sessionPageNumber)
        {
            return new NewPrompt(
                game,
                gamePageNumber,
                sessionPageNumber);
        }

        private record NewPrompt(
            Game Game,
            int GamePageNumber,
            int SessionPageNumber) : INewPrompt;
    }

    internal interface IDataError
    {
        static IDataError Create()
        {
            return new DataError();
        }

        private record DataError : IDataError;
    }

    internal interface INewSession
    {
        string SessionName { get; }
        Game Game { get; }
        Session[] Sessions { get; }
        int SessionsCount { get; }
        int GamePageNumber { get; }
        int SessionPageNumber { get; }
        int SessionPageCount { get; }

        static INewSession Create(
            string sessionName,
            Game game,
            Session[] sessions,
            int sessionsCount,
            int gamePageNumber,
            int sessionPageNumber,
            int sessionPageCount)
        {
            return new NewSession(
                sessionName,
                game,
                sessions,
                sessionsCount,
                gamePageNumber,
                sessionPageNumber,
                sessionPageCount);
        }

        private record NewSession(
            string SessionName,
            Game Game,
            Session[] Sessions,
            int SessionsCount,
            int GamePageNumber,
            int SessionPageNumber,
            int SessionPageCount) : INewSession;
    }
}