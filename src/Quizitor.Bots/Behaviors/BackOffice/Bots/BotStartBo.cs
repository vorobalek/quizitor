using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Services.TelegramBot.ClientFactory;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Extensions;
using Context = Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix.ICallbackQueryDataPrefixContext<Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure.IBotBackOfficeContext>;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BotStartBo(
    ITelegramBotClientFactory telegramBotClientFactory,
    IDbContextProvider dbContextProvider) :
    BotViewBo(
        dbContextProvider)
{
    /// <summary>
    ///     <b>botstart</b>.{botId}.{botPageNumber}
    /// </summary>
    public new const string Button = "botstart";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeBotView,
        UserPermission.BackOfficeBotStart
    ];

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (await StartAsync(
                context.Base.Bot,
                context,
                cancellationToken))
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await ResponseAsync(context, cancellationToken));
        else
            await ResponseAsync(context, cancellationToken);
    }

    public async Task<bool> StartAsync(
        Bot bot,
        ICallbackQueryDataPrefixContext<IBehaviorContext> context,
        CancellationToken cancellationToken)
    {
        if (bot.IsActive) return false;
        var botClientWrapper = telegramBotClientFactory.CreateForBot(bot);

        var botCommands = await _dbContextProvider
            .BotCommands
            .GetByTypeAsync(
                bot.Type,
                cancellationToken);

        await botClientWrapper
            .SetMyCommands(
                botCommands.Select(BotCommandExtensions.ToTelegramBotCommand),
                cancellationToken: cancellationToken);

        var botUser = await botClientWrapper.GetMe(cancellationToken);

        bot.IsActive = true;
        bot.Username = botUser.Username;
        await _dbContextProvider
            .Bots
            .UpdateAsync(bot, cancellationToken);

        _dbContextProvider
            .AddPostCommitTask(async () =>
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string.Format(
                            TR.L + "_BACKOFFICE_BOT_STARTED_TXT",
                            bot.Name),
                        true,
                        cancellationToken: cancellationToken));

        return true;
    }
}