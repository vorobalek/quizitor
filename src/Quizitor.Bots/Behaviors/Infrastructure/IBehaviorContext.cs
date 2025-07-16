using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.Services.TelegramBot.ClientWrapper;
using Quizitor.Data.Entities;
using Quizitor.Kafka.Contracts;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal interface IBehaviorContext
{
    UpdateContext UpdateContext { get; }
    TelegramUser TelegramUser { get; }
    Bot? EntryBot { get; }
    ITelegramBotClientWrapper Client { get; }
    string? QrData { get; }
    IIdentity Identity { get; }

    static IBehaviorContext Create(
        UpdateContext updateContext,
        TelegramUser telegramUser,
        Bot? entryBot,
        ITelegramBotClientWrapper client,
        string? qrData,
        IIdentity identity)
    {
        return new BehaviorContext(
            updateContext,
            telegramUser,
            entryBot,
            client,
            qrData,
            identity);
    }

    private record BehaviorContext(
        UpdateContext UpdateContext,
        TelegramUser TelegramUser,
        Bot? EntryBot,
        ITelegramBotClientWrapper Client,
        string? QrData,
        IIdentity Identity) : IBehaviorContext;
}