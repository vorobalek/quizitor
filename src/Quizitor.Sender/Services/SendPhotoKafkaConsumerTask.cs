using Microsoft.Extensions.Options;
using Prometheus;
using Quizitor.Common;
using Quizitor.Kafka;
using Quizitor.Sender.Services.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace Quizitor.Sender.Services;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed partial class SendPhotoKafkaConsumerTask(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<KafkaOptions> options,
    ILogger<SendPhotoKafkaConsumerTask> logger)
    : SlaSenderKafkaConsumerTask<long>(
        serviceScopeFactory,
        options,
        logger)
{
    private static readonly Histogram SenderSendPhotoBackOfficeHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_send_photo_backoffice",
        "Histogram of backoffice sendPhoto call.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "is_test"
            ]
        });

    private static readonly Histogram SenderSendPhotoBotHistogram = Metrics.CreateHistogram(
        $"{Constants.MetricsPrefix}sender_send_photo_bot",
        "Histogram of bot's sendPhoto call.",
        new HistogramConfiguration
        {
            Buckets =
            [
                .05, .1, .2, .3, .4, .5, .6, .7, .8, .9, 1, 2, 5, 10, 30, 60
            ],
            LabelNames =
            [
                "bot_id",
                "is_test"
            ]
        });

    protected override string Method => "sendPhoto";
    protected override string TopicMain => KafkaTopics.SendPhotoTopicName;
    protected override string TopicBot => KafkaTopics.SendPhotoBotTopicName;
    protected override Histogram BackOfficeHistogram => SenderSendPhotoBackOfficeHistogram;
    protected override Histogram BotHistogram => SenderSendPhotoBotHistogram;

    protected override async Task SendRequestAsync(
        HttpClient httpClient,
        string botToken,
        string content,
        CancellationToken cancellationToken)
    {
        var request = JsonSerializerHelper.TryDeserialize<SendPhotoRequest>(content, JsonBotAPI.Options);
        if (request is not
            {
                Photo: InputFileUrl
                {
                    Url:
                    {
                        IsFile: true
                    } fileUrl
                }
            } ||
            !File.Exists(fileUrl.AbsolutePath))
            return;

        await using var fileStream = File.OpenRead(fileUrl.AbsolutePath);
        request.Photo = fileStream;
        var client = new TelegramBotClient(botToken, httpClient, cancellationToken);
        try
        {
            await client.SendRequest(request, cancellationToken);
        }
        catch (ApiRequestException exception)
        {
            LogAnExceptionOccurredWhileSendingAPhoto(logger, exception);
            if (exception.ErrorCode == 429)
                throw new RetryLaterExtension(1000, "429 status code has been found");
        }
    }

    [LoggerMessage(LogLevel.Error, "An exception occurred while sending a photo")]
    static partial void LogAnExceptionOccurredWhileSendingAPhoto(ILogger logger, Exception exception);
}