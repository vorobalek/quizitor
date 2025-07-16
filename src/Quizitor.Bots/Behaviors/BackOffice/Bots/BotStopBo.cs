using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Context = Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix.ICallbackQueryDataPrefixContext<Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure.IBotBackOfficeContext>;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

// ReSharper disable once ClassNeverInstantiated.Global
internal class BotStopBo(IDbContextProvider dbContextProvider) : BotViewBo(dbContextProvider)
{
    /// <summary>
    ///     <b>botstop</b>.{botId}.{botPageNumber}
    /// </summary>
    public new const string Button = "botstop";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeBotView,
        UserPermission.BackOfficeBotStop
    ];

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (await StopAsync(
                context.Base.Bot,
                context,
                cancellationToken))
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await ResponseAsync(context, cancellationToken));
        else
            await ResponseAsync(context, cancellationToken);
    }

    public async Task<bool> StopAsync(
        Bot bot,
        ICallbackQueryDataPrefixContext<IBehaviorContext> context,
        CancellationToken cancellationToken)
    {
        if (!bot.IsActive) return false;

        bot.IsActive = false;
        await _dbContextProvider
            .Bots
            .UpdateAsync(
                bot,
                cancellationToken);

        _dbContextProvider
            .AddPostCommitTask(async () =>
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string.Format(
                            TR.L + "_BACKOFFICE_BOT_STOPPED_TXT",
                            bot.Name),
                        true,
                        cancellationToken: cancellationToken));
        return true;
    }
}