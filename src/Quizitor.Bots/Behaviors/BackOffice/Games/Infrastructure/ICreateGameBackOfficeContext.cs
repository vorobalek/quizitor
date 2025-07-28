using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Infrastructure;

internal interface ICreateGameBackOfficeContext : IBackOfficeContext
{
    IWrongPrompt? WrongPrompt { get; }
    INewPrompt? NewPrompt { get; }
    IDataError? DataError { get; }
    INewGame? NewGame { get; }

    static ICreateGameBackOfficeContext Create(
        IWrongPrompt? wrongPrompt,
        INewPrompt? newPrompt,
        IDataError? dataError,
        INewGame? newGame,
        IBackOfficeContext baseContext)
    {
        return new CreateGameBackOfficeContext(
            wrongPrompt,
            newPrompt,
            dataError,
            newGame,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record CreateGameBackOfficeContext(
        IWrongPrompt? WrongPrompt,
        INewPrompt? NewPrompt,
        IDataError? DataError,
        INewGame? NewGame,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ICreateGameBackOfficeContext;

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
        int GamePageNumber { get; }

        static INewPrompt Create(int gamePageNumber)
        {
            return new NewPrompt(gamePageNumber);
        }

        private record NewPrompt(int GamePageNumber) : INewPrompt;
    }

    internal interface IDataError
    {
        static IDataError Create()
        {
            return new DataError();
        }

        private record DataError : IDataError;
    }

    internal interface INewGame
    {
        string GameTitle { get; }
        Game[] Games { get; }
        int GamesCount { get; }
        int GamePageNumber { get; }
        int GamePageCount { get; }

        static INewGame Create(
            string gameTitle,
            Game[] games,
            int gamesCount,
            int gamePageNumber,
            int gamePageCount)
        {
            return new NewGame(
                gameTitle,
                games,
                gamesCount,
                gamePageNumber,
                gamePageCount);
        }

        private record NewGame(
            string GameTitle,
            Game[] Games,
            int GamesCount,
            int GamePageNumber,
            int GamePageCount) : INewGame;
    }
}