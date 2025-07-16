using Quizitor.Kafka;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.DeleteMessage;

internal interface IDeleteMessageKafkaProducer : ISenderKafkaProducer<DeleteMessageRequest>;