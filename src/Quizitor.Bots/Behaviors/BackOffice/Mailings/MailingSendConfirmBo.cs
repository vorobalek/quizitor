using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Services.Kafka.Producers.SendMessage;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

// ReSharper disable once ClassNeverInstantiated.Global
internal class MailingSendConfirmBo(
    ISendMessageKafkaProducer sendMessageKafkaProducer,
    IDbContextProvider dbContextProvider) :
    MailingProfileBo(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingsendconfirm</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public new const string Button = "mailingsendconfirm";

    public override string[] Permissions => [UserPermission.BackOfficeMailingSend];

    protected override string ButtonInline => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<IMailingProfileBackOfficeContext> context,
        CancellationToken cancellationToken)
    {
        foreach (var userSchema in context.Base.Schema)
        {
            var userId = userSchema.Key.Id;
            var botIds = userSchema.Select(x => (int?)x.Id).ToArray();
            await sendMessageKafkaProducer
                .ProduceBatchAsync(
                    new SendMessageRequest
                    {
                        ChatId = userId,
                        Text = context.Base.Mailing.Text,
                        ParseMode = ParseMode.Html
                    },
                    context.Base.UpdateContext,
                    cancellationToken,
                    botIds);
        }

        await context
            .Base
            .Client
            .AnswerCallbackQuery(
                context.Base.UpdateContext,
                context.CallbackQueryId,
                string.Format(
                    TR.L + "_BACKOFFICE_MAILING_SENT_CLB",
                    context.Base.Mailing.Name,
                    context.Base.PredictedMessagesCount,
                    context.Base.PredictedUsersCount,
                    context.Base.PredictedBotsCount),
                true,
                cancellationToken: cancellationToken);

        await base.PerformCallbackQueryDataPrefixAsync(context, cancellationToken);
    }
}