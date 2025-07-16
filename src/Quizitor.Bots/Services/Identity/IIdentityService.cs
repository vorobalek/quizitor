using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.Identity;

internal interface IIdentityService
{
    Task<IIdentity> IdentifyAsync(
        TelegramUser telegramUser,
        Bot? bot,
        DateTimeOffset serverTime,
        CancellationToken cancellationToken);
}