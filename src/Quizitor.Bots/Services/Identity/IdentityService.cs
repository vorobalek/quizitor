using Quizitor.Bots.Configuration;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Services.Identity;

internal sealed class IdentityService(
    IDbContextProvider dbContextProvider,
    ITelegramBotClientFactory telegramBotClientFactory) : IIdentityService
{
    private static string? _globalUsername;

    public async Task<IIdentity> IdentifyAsync(
        TelegramUser telegramUser,
        Bot? bot,
        DateTimeOffset serverTime,
        CancellationToken cancellationToken)
    {
        var user = await CreateOrUpdateUserAsync(
            telegramUser,
            cancellationToken);

        var hasFullAccess =
            !TelegramBotConfiguration.IsSaUserAuthorizationEnabled ||
            TelegramBotConfiguration.AuthorizedUserIds.Contains(telegramUser.Id);

        var prompt = await GetPromptAsync(
            user,
            bot,
            cancellationToken);

        var permissions = await GetPermissionsAsync(
            user,
            cancellationToken);

        await UpsertBotInteractionAsync(
            user,
            bot,
            serverTime,
            cancellationToken);

        return new Identity(
            user,
            hasFullAccess,
            prompt,
            permissions);
    }

    private async Task<User> CreateOrUpdateUserAsync(
        TelegramUser telegramUser,
        CancellationToken cancellationToken)
    {
        if (await GetUserAsync(
                telegramUser,
                cancellationToken) is not { } user)
            user = await CreateUserAsync(
                telegramUser,
                cancellationToken);
        else
            await UpdateUserAsync(
                user,
                telegramUser,
                cancellationToken);

        return user;
    }

    private Task<User?> GetUserAsync(
        TelegramUser telegramUser,
        CancellationToken cancellationToken)
    {
        return dbContextProvider
            .Users
            .GetByIdOrDefaultAsync(
                telegramUser.Id,
                cancellationToken);
    }

    private async Task<User> CreateUserAsync(
        TelegramUser telegramUser,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = telegramUser.Id,
            FirstName = telegramUser.FirstName,
            LastName = telegramUser.LastName,
            Username = telegramUser.Username
        };

        await dbContextProvider
            .Users
            .AddAsync(
                user,
                cancellationToken);

        return user;
    }

    private async Task UpdateUserAsync(
        User user,
        TelegramUser telegramUser,
        CancellationToken cancellationToken)
    {
        var needsUpdate = false;
        if (user.FirstName != telegramUser.FirstName)
        {
            user.FirstName = telegramUser.FirstName;
            needsUpdate = true;
        }

        if (user.LastName != telegramUser.LastName)
        {
            user.LastName = telegramUser.LastName;
            needsUpdate = true;
        }

        if (user.Username != telegramUser.Username)
        {
            user.Username = telegramUser.Username;
            needsUpdate = true;
        }

        if (needsUpdate)
            await dbContextProvider
                .Users
                .UpdateAsync(
                    user,
                    cancellationToken);
    }

    private Task<UserPrompt?> GetPromptAsync(
        User user,
        Bot? bot,
        CancellationToken cancellationToken)
    {
        return dbContextProvider
            .Users
            .GetPromptByUserIdBotIdOrDefaultAsync(
                user.Id,
                bot?.Id,
                cancellationToken);
    }

    private Task<string[]> GetPermissionsAsync(
        User user,
        CancellationToken cancellationToken)
    {
        return dbContextProvider
            .Users
            .GetAllPermissionsAsync(
                user.Id,
                cancellationToken);
    }

    private async Task UpsertBotInteractionAsync(
        User user,
        Bot? bot,
        DateTimeOffset serverTime,
        CancellationToken cancellationToken)
    {
        var username = (bot is not null
                           ? bot.Username
                           : _globalUsername ??= (await telegramBotClientFactory
                                   .CreateDefault()
                                   .GetMe(cancellationToken))
                               .Username)
                       ?? throw new InvalidOperationException("Bot username is not ready yet.");

        await dbContextProvider
            .BotInteractions
            .UpsertAsync(
                username,
                user.Id,
                serverTime,
                cancellationToken);
    }

    private record Identity(
        User User,
        bool HasFullAccess,
        UserPrompt? Prompt,
        string[] Permissions) : IIdentity;
}