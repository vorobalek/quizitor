using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;

internal interface ICreateMailingBackOfficeContext : IBackOfficeContext
{
    IWrongPrompt? WrongPrompt { get; }
    INewPrompt? NewPrompt { get; }
    IDataError? DataError { get; }
    INewMailingName? NewMailingName { get; }
    INewMailingText? NewMailingText { get; }

    static ICreateMailingBackOfficeContext Create(
        IWrongPrompt? wrongPrompt,
        INewPrompt? newPrompt,
        IDataError? dataError,
        INewMailingName? newMailingName,
        INewMailingText? newMailingText,
        IBackOfficeContext baseContext)
    {
        return new CreateMailingBackOfficeContext(
            wrongPrompt,
            newPrompt,
            dataError,
            newMailingName,
            newMailingText,
            baseContext.UpdateContext,
            baseContext.TelegramUser,
            baseContext.EntryBot,
            baseContext.Client,
            baseContext.QrData,
            baseContext.Identity);
    }

    private record CreateMailingBackOfficeContext(
        IWrongPrompt? WrongPrompt,
        INewPrompt? NewPrompt,
        IDataError? DataError,
        INewMailingName? NewMailingName,
        INewMailingText? NewMailingText,
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : ICreateMailingBackOfficeContext;

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
        int MailingPageNumber { get; }

        static INewPrompt Create(int mailingPageNumber)
        {
            return new NewPrompt(mailingPageNumber);
        }

        private record NewPrompt(int MailingPageNumber) : INewPrompt;
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

    internal interface INewMailingName
    {
        string MailingName { get; }
        int MailingPageNumber { get; }

        static INewMailingName Create(
            string mailingName,
            int mailingPageNumber)
        {
            return new NewMailingName(
                mailingName,
                mailingPageNumber);
        }

        private record NewMailingName(
            string MailingName,
            int MailingPageNumber) : INewMailingName;
    }

    internal interface INewMailingText : INewMailingName
    {
        string MailingText { get; }
        Mailing[] Mailings { get; }
        int MailingPageCount { get; }

        static INewMailingText Create(
            string mailingName,
            string mailingText,
            Mailing[] mailings,
            int mailingPageNumber,
            int mailingPageCount)
        {
            return new NewMailingText(
                mailingName,
                mailingText,
                mailings,
                mailingPageNumber,
                mailingPageCount);
        }

        private record NewMailingText(
            string MailingName,
            string MailingText,
            Mailing[] Mailings,
            int MailingPageNumber,
            int MailingPageCount) : INewMailingText;
    }
}