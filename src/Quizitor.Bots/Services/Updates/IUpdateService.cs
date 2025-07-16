using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Services.Updates;

internal interface IUpdateService
{
    Task HandleAsync(
        UpdateContext updateContext,
        Bot? bot,
        CancellationToken cancellationToken);
}