using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;
using Game = Quizitor.Data.Entities.Game;

namespace Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Infrastructure;

internal interface ICreateRoundBackOfficeContext : IBackOfficeContext
{
    IWrongPrompt? WrongPrompt { get; }
    INewPrompt? NewPrompt { get; }
    IDataError? DataError { get; }
    INewRound? NewRound { get; }

    static ICreateRoundBackOfficeContext Create(
        IWrongPrompt? wrongPrompt,
        INewPrompt? newPrompt,
        IDataError? dataError,
        INewRound? newRound,
        IBackOfficeContext baseContext)
    {
        return new CreateRoundBackOfficeContext(
            wrongPrompt,
            newPrompt,
            dataError,
            newRound,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record CreateRoundBackOfficeContext(
        IWrongPrompt? WrongPrompt,
        INewPrompt? NewPrompt,
        IDataError? DataError,
        INewRound? NewRound,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ICreateRoundBackOfficeContext;

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
        int RoundPageNumber { get; }

        static INewPrompt Create(
            Game game,
            int gamePageNumber,
            int roundPageNumber)
        {
            return new NewPrompt(
                game,
                gamePageNumber,
                roundPageNumber);
        }

        private record NewPrompt(
            Game Game,
            int GamePageNumber,
            int RoundPageNumber) : INewPrompt;
    }

    internal interface IDataError
    {
        static IDataError Create()
        {
            return new DataError();
        }

        private record DataError : IDataError;
    }

    internal interface INewRound
    {
        string RoundTitle { get; }
        Game Game { get; }
        Round[] Rounds { get; }
        int RoundNumber { get; }
        int GamePageNumber { get; }
        int RoundPageNumber { get; }
        int RoundPageCount { get; }

        static INewRound Create(
            string roundTitle,
            Game game,
            Round[] rounds,
            int roundNumber,
            int gamePageNumber,
            int roundPageNumber,
            int roundPageCount)
        {
            return new NewRound(
                roundTitle,
                game,
                rounds,
                roundNumber,
                gamePageNumber,
                roundPageNumber,
                roundPageCount);
        }

        private record NewRound(
            string RoundTitle,
            Game Game,
            Round[] Rounds,
            int RoundNumber,
            int GamePageNumber,
            int RoundPageNumber,
            int RoundPageCount) : INewRound;
    }
}