using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Context = Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix.ICallbackQueryDataPrefixContext<Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure.IBotBackOfficeContext>;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BotTypeBo(
    IDbContextProvider dbContextProvider) :
    BotStopBo(dbContextProvider)
{
    /// <summary>
    ///     <b>bottype</b>.{botId}.{botPageNumber}
    /// </summary>
    public new const string Button = "bottype";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeBotView,
        UserPermission.BackOfficeBotEdit,
        UserPermission.BackOfficeBotStop
    ];

    protected override string ButtonInternal => Button;

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        context.Base.Bot.Type = (BotType)((Convert.ToInt32(context.Base.Bot.Type) + 1) % (Enum.GetValues<BotType>().Length - 1));

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