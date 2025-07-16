using Quizitor.Kafka;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.AnswerCallbackQuery;

internal interface IAnswerCallbackQueryKafkaProducer : ISenderKafkaProducer<AnswerCallbackQueryRequest>;