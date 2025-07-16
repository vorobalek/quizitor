using Quizitor.Kafka;
using Telegram.Bot.Requests;

namespace Quizitor.Bots.Services.Kafka.Producers.SendPhoto;

internal interface ISendPhotoKafkaProducer : ISenderKafkaProducer<SendPhotoRequest>;