using System.Text.Json;
using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.UI.Shared;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

using CallbackQueryBehavior = ICallbackQueryDataPrefixBehaviorTrait<ICreateMailingBackOfficeContext>;
using CallbackQueryContext = ICallbackQueryDataPrefixContext<ICreateMailingBackOfficeContext>;
using MessageBehavior = IMessageTextBehaviorTrait<ICreateMailingBackOfficeContext>;
using MessageContext = IMessageTextContext<ICreateMailingBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CreateMailingBo(IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<ICreateMailingBackOfficeContext>,
    CallbackQueryBehavior,
    MessageBehavior
{
    /// <summary>
    ///     <b>createmailing</b>.{mailingPageNumber}
    /// </summary>
    public const string Button = "createmailing";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeMailingCreate,
        UserPermission.BackOfficeMailingList
    ];

    public string CallbackQueryDataPrefixValue => $"{Button}.";

    public async Task PerformCallbackQueryDataPrefixAsync(
        CallbackQueryContext context,
        CancellationToken cancellationToken)
    {
        if (context.Base.WrongPrompt is { } wrongPrompt)
        {
            await FallbackWrongUserPromptAsync(
                context,
                wrongPrompt.PromptType,
                cancellationToken);
            return;
        }

        if (context.Base.NewPrompt is { } newPrompt)
        {
            await dbContextProvider
                .Users
                .SetPromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    UserPromptType.BackOfficeNewMailingName,
                    JsonSerializer.Serialize(
                        new NamePromptSubject(newPrompt.MailingPageNumber)),
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_MESSAGE_{UserPromptType.BackOfficeNewMailingName}_TXT",
                            ParseMode.Html,
                            replyMarkup: Keyboards.CancelPrompt,
                            cancellationToken: cancellationToken));
        }
    }

    public override bool ShouldPerformMessageText(IBehaviorContext baseContext)
    {
        return !IMessageTextBotCommandEqualsContext.IsValidUpdate(baseContext.UpdateContext.Update) &&
               base.ShouldPerformMessageText(baseContext) &&
               baseContext.Identity.Prompt?.Type is UserPromptType.BackOfficeNewMailingName or UserPromptType.BackOfficeNewMailingText;
    }

    public async Task PerformMessageTextAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        if (context.Base.DataError is { } dataError)
        {
            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            dataError.Error != null
                                ? string.Format(
                                    TR.L + "_BACKOFFICE_MAILING_NOT_CREATED_ERROR_TXT",
                                    dataError.Error)
                                : TR.L + "_BACKOFFICE_MAILING_NOT_CREATED_UNKNOWN_ERROR_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
            return;
        }

        if (context.Base.NewMailingName is { } newMailingName)
        {
            await dbContextProvider
                .Users
                .SetPromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    UserPromptType.BackOfficeNewMailingText,
                    JsonSerializer.Serialize(
                        new TextPromptSubject(
                            newMailingName.MailingName,
                            newMailingName.MailingPageNumber)),
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_MESSAGE_{UserPromptType.BackOfficeNewMailingText}_TXT",
                            ParseMode.Html,
                            replyMarkup: Keyboards.CancelPrompt,
                            cancellationToken: cancellationToken));
        }

        if (context.Base.NewMailingText is { } newMailingText)
        {
            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);

            var mailing = new Mailing
            {
                Name = newMailingText.MailingName,
                Text = newMailingText.MailingText
            };
            await dbContextProvider
                .Mailings
                .AddAsync(
                    mailing,
                    cancellationToken);

            var mailingListContext = IMessageTextContext.Create(
                IMailingListBackOfficeContext.Create(
                    [mailing, .. newMailingText.Mailings],
                    newMailingText.MailingsCount,
                    newMailingText.MailingPageNumber,
                    newMailingText.MailingPageCount,
                    context.Base));

            dbContextProvider
                .AddPostCommitTask(async () =>
                {
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            string
                                .Format(
                                    TR.L + "_BACKOFFICE_MAILING_CREATED_TXT",
                                    mailing.Name.EscapeHtml(),
                                    mailing.Text.EscapeHtml()),
                            ParseMode.Html,
                            cancellationToken: cancellationToken);

                    await MailingListBo.ResponseAsync(mailingListContext, cancellationToken);
                });
        }
    }

    protected override async Task<ICreateMailingBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        if (ICallbackQueryDataPrefixContext.IsValidUpdate(backOfficeContext.UpdateContext.Update, CallbackQueryDataPrefixValue))
        {
            var callbackQueryContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);

            if (callbackQueryContext?.Base.Identity.Prompt?.Type is { } promptType)
            {
                return ICreateMailingBackOfficeContext.Create(
                    ICreateMailingBackOfficeContext.IWrongPrompt.Create(promptType),
                    null,
                    null,
                    null,
                    null,
                    backOfficeContext);
            }

            if (callbackQueryContext?.CallbackQueryDataPostfix.Split('.') is
                [
                    {
                    } mailingPageNumberString
                ] &&
                int.TryParse(mailingPageNumberString, out var mailingPageNumber))
            {
                return ICreateMailingBackOfficeContext.Create(
                    null,
                    ICreateMailingBackOfficeContext.INewPrompt.Create(mailingPageNumber),
                    null,
                    null,
                    null,
                    callbackQueryContext.Base);
            }
        }

        if (IMessageTextContext.IsValidUpdate(backOfficeContext.UpdateContext.Update))
        {
            if (backOfficeContext.Identity.Prompt?.Type is not UserPromptType.BackOfficeNewMailingName and not UserPromptType.BackOfficeNewMailingText) return null;
            var messageTextContext = IMessageTextContext.Create(backOfficeContext);

            if (backOfficeContext.Identity.Prompt?.Type is UserPromptType.BackOfficeNewMailingName)
            {
                if (messageTextContext?.Base.Identity.Prompt?.Subject is not { } promptSubjectString ||
                    JsonSerializerHelper.TryDeserialize<NamePromptSubject>(promptSubjectString) is not
                    {
                        MailingPageNumber: { } mailingPageNumber
                    })
                {
                    return ICreateMailingBackOfficeContext.Create(
                        null,
                        null,
                        ICreateMailingBackOfficeContext.IDataError.Create(null),
                        null,
                        null,
                        backOfficeContext);
                }

                var mailingName = messageTextContext.MessageText;
                if (mailingName.Length > 256)
                {
                    return ICreateMailingBackOfficeContext.Create(
                        null,
                        null,
                        ICreateMailingBackOfficeContext.IDataError.Create(TR.L + "_BACKOFFICE_MAILING_NAME_TOO_LONG_TXT"),
                        null,
                        null,
                        backOfficeContext);
                }

                return ICreateMailingBackOfficeContext.Create(
                    null,
                    null,
                    null,
                    ICreateMailingBackOfficeContext.INewMailingName.Create(
                        mailingName,
                        mailingPageNumber),
                    null,
                    backOfficeContext);
            }

            if (backOfficeContext.Identity.Prompt?.Type is UserPromptType.BackOfficeNewMailingText)
            {
                if (messageTextContext?.Base.Identity.Prompt?.Subject is not { } promptSubjectString ||
                    JsonSerializerHelper.TryDeserialize<TextPromptSubject>(promptSubjectString) is not
                    {
                        MailingName: { } mailingName,
                        MailingPageNumber: { } mailingPageNumber
                    })
                {
                    return ICreateMailingBackOfficeContext.Create(
                        null,
                        null,
                        ICreateMailingBackOfficeContext.IDataError.Create(null),
                        null,
                        null,
                        backOfficeContext);
                }

                var mailingText = messageTextContext.MessageText;
                if (mailingText.Length > 4096)
                {
                    return ICreateMailingBackOfficeContext.Create(
                        null,
                        null,
                        ICreateMailingBackOfficeContext.IDataError.Create(TR.L + "_BACKOFFICE_MAILING_TEXT_TOO_LONG_TXT"),
                        null,
                        null,
                        backOfficeContext);
                }

                var mailings = await dbContextProvider
                    .Mailings
                    .GetPaginatedAfterCreationAsync(
                        MailingListBo.PageSize,
                        cancellationToken);

                var mailingsCount = await dbContextProvider
                    .Mailings
                    .CountAsync(cancellationToken) + 1;

                var mailingPageCount = Convert.ToInt32(
                    Math.Ceiling(
                        Convert.ToDouble(mailingsCount) / MailingListBo.PageSize));

                return ICreateMailingBackOfficeContext.Create(
                    null,
                    null,
                    null,
                    null,
                    ICreateMailingBackOfficeContext.INewMailingText.Create(
                        mailingName,
                        mailingText,
                        mailings,
                        mailingsCount,
                        mailingPageNumber,
                        mailingPageCount),
                    backOfficeContext);
            }
        }

        return null;
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record NamePromptSubject(int? MailingPageNumber);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record TextPromptSubject(
        string? MailingName,
        int? MailingPageNumber);
}