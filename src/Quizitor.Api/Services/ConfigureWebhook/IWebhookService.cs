using Quizitor.Data.Entities;

namespace Quizitor.Api.Services.ConfigureWebhook;

internal interface IWebhookService
{
    Task<TelegramUser> SetForBotAsync(Bot bot, CancellationToken cancellationToken);

    Task<TelegramUser> SetDefaultAsync(CancellationToken cancellationToken);

    Task<TelegramUser> DeleteForBotAsync(Bot bot, CancellationToken cancellationToken);

    Task<TelegramUser> DeleteDefaultAsync(CancellationToken cancellationToken);
}