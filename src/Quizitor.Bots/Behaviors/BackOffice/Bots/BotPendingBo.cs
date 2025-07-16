using Quizitor.Data;
using Quizitor.Data.Entities;
using Context = Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix.ICallbackQueryDataPrefixContext<Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure.IBotBackOfficeContext>;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BotPendingBo(
    IDbContextProvider dbContextProvider) :
    BotViewBo(
        dbContextProvider)
{
    /// <summary>
    ///     <b>botpending</b>.{botId}.{botPageNumber}
    /// </summary>
    public new const string Button = "botpending";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeBotView,
        UserPermission.BackOfficeBotEdit
    ];

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        context.Base.Bot.DropPendingUpdates = !context.Base.Bot.DropPendingUpdates;
        await _dbContextProvider
            .Bots
            .UpdateAsync(
                context.Base.Bot,
                cancellationToken);
        _dbContextProvider
            .AddPostCommitTask(async () =>
                await ResponseAsync(context, cancellationToken));
    }
}