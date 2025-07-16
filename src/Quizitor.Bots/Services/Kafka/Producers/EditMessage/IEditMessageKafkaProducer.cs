using Quizitor.Kafka;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.EditMessage;

internal interface IEditMessageKafkaProducer : ISenderKafkaProducer<EditMessageTextRequest>;