using Quizitor.Kafka;
using Telegram.Bot.Requests;

namespace Quizitor.Api.Services.Kafka.SendChatAction;

internal interface ISendChatActionKafkaProducer : ISenderKafkaProducer<SendChatActionRequest>;