using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Services.Crypto;
using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.Qr;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Services.Updates;

internal sealed class UpdateService(
    IDbContextProvider dbContextProvider,
    ITelegramBotClientFactory telegramBotClientFactory,
    IQrService qrService,
    IEncryptionService encryptionService,
    IIdentityService identityService,
    IEnumerable<IBehavior> behaviors) : IUpdateService
{
    public async Task HandleAsync(
        UpdateContext updateContext,
        Bot? bot,
        CancellationToken cancellationToken)
    {
        var telegramUser = updateContext.Update.GetUser();
        var client = GetBotClient(bot);

        var qrData = await qrService.TryExtractFromUpdateAndSaveFileAsync(
            client,
            bot?.Id,
            updateContext.Update,
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(qrData))
            qrData = encryptionService.TryDecryptString(qrData);

        var identity = await identityService.IdentifyAsync(
            telegramUser,
            bot,
            updateContext.InitiatedAt ?? await dbContextProvider.GetServerDateTimeOffsetAsync(cancellationToken),
            cancellationToken);

        var context = IBehaviorContext.Create(
            updateContext,
            telegramUser,
            bot,
            client,
            qrData,
            identity);

        await HandleUpdateAsync(
            bot?.Type ?? BotType.BackOffice,
            context,
            cancellationToken);
    }

    private ITelegramBotClientWrapper GetBotClient(Bot? bot)
    {
        return bot is null
            ? telegramBotClientFactory.CreateDefault()
            : telegramBotClientFactory.CreateForBot(bot);
    }

    private async Task HandleUpdateAsync(
        BotType botType,
        IBehaviorContext context,
        CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            behaviors
                .Where(x =>
                    (x.Type == botType ||
                     x.Type == BotType.Universal) &&
                    x.ShouldPerform(context))
                .Select(x => x
                    .PerformAsync(
                        context,
                        cancellationToken)));
    }
}