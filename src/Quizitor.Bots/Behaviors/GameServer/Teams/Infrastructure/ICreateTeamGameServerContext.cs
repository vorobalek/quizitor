using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.GameServer.Teams.Infrastructure;

internal interface ICreateTeamGameServerContext : IGameServerContext
{
    IWrongPrompt? WrongPrompt { get; }
    INewPrompt? NewPrompt { get; }
    IDataError? DataError { get; }
    INewTeam? NewTeam { get; }

    static ICreateTeamGameServerContext Create(
        IWrongPrompt? wrongPrompt,
        INewPrompt? newPrompt,
        IDataError? dataError,
        INewTeam? newTeam,
        IGameServerContext baseContext)
    {
        return new CreateTeamGameServerContext(
            wrongPrompt,
            newPrompt,
            dataError,
            newTeam,
            baseContext.SessionTeamInfo,
            baseContext.Game,
            baseContext.Session,
            baseContext.TargetBot,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record CreateTeamGameServerContext(
        IWrongPrompt? WrongPrompt,
        INewPrompt? NewPrompt,
        IDataError? DataError,
        INewTeam? NewTeam,
        ISessionTeamInfo? SessionTeamInfo,
        Game Game,
        Session Session,
        Bot? TargetBot,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ICreateTeamGameServerContext;

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
        int TeamPageNumber { get; }

        static INewPrompt Create(int teamPageNumber)
        {
            return new NewPrompt(teamPageNumber);
        }

        private record NewPrompt(int TeamPageNumber) : INewPrompt;
    }

    internal interface IDataError
    {
        string? Error { get; }

        static IDataError Create(string? error)
        {
            return new DataError(error);
        }

        private record DataError(string? Error) : IDataError;
    }

    internal interface INewTeam
    {
        string TeamName { get; }
        Team[] Teams { get; }
        int TeamPageNumber { get; }
        int TeamPageCount { get; }

        static INewTeam Create(
            string teamName,
            Team[] teams,
            int teamPageNumber,
            int teamPageCount)
        {
            return new NewTeam(
                teamName,
                teams,
                teamPageNumber,
                teamPageCount);
        }

        private record NewTeam(
            string TeamName,
            Team[] Teams,
            int TeamPageNumber,
            int TeamPageCount) : INewTeam;
    }
}