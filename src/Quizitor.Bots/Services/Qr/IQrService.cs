using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Telegram.Bot.Types;

namespace Quizitor.Bots.Services.Qr;

internal interface IQrService
{
    Task<string?> TryExtractFromUpdateAndSaveFileAsync(
        ITelegramBotClientWrapper client,
        int? botId,
        Update update,
        CancellationToken cancellationToken);

    Task<string> GenerateFromStringIfNeededAndSaveFileAsync(
        string data,
        string fileName,
        CancellationToken cancellationToken,
        bool forceRegenerate = false,
        string? textUp = null,
        string? textDown = null);
}