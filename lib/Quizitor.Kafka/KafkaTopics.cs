namespace Quizitor.Kafka;

public static class KafkaTopics
{
    private const string Prefix = "Quizitor.";
    public const string UpdateTopicName = $"{Prefix}Update";
    public const string UpdateBotTopicName = $"{Prefix}Update.{{0}}";
    public const string SendChatActionTopicName = $"{Prefix}SendChatAction";
    public const string SendChatActionBotTopicName = $"{Prefix}SendChatAction.{{0}}";
    public const string SendMessageTopicName = $"{Prefix}SendMessage";
    public const string SendMessageBotTopicName = $"{Prefix}SendMessage.{{0}}";
    public const string SendPhotoTopicName = $"{Prefix}SendPhoto";
    public const string SendPhotoBotTopicName = $"{Prefix}SendPhoto.{{0}}";
    public const string EditMessageTopicName = $"{Prefix}EditMessage";
    public const string EditMessageBotTopicName = $"{Prefix}EditMessage.{{0}}";
    public const string AnswerCallbackQueryTopicName = $"{Prefix}AnswerCallbackQuery";
    public const string AnswerCallbackQueryBotTopicName = $"{Prefix}AnswerCallbackQuery.{{0}}";
    public const string DeleteMessageTopicName = $"{Prefix}DeleteMessage";
    public const string DeleteMessageBotTopicName = $"{Prefix}DeleteMessage.{{0}}";
    public const string QuestionTimingNotifyTopicName = $"{Prefix}QuestionTimingNotify";
    public const string QuestionTimingStopTopicName = $"{Prefix}QuestionTimingStop";
}