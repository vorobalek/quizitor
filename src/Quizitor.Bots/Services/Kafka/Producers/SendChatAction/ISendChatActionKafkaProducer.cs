using Quizitor.Kafka;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.SendChatAction;

internal interface ISendChatActionKafkaProducer : ISenderKafkaProducer<SendChatActionRequest>;