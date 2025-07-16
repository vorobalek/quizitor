using Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots;

using Context = ICallbackQueryDataPrefixContext<IBotListCommandBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class BotListStartBo(
    BotStartBo botStartBo,
    IDbContextProvider dbContextProvider) :
    BotListCommandBo(
        dbContextProvider)
{
    /// <summary>
    ///     <b>bots</b>.{botPageNumber}.<b>start</b>.{botId}
    /// </summary>
    public const string Command = "start";

    public override string[] Permissions =>
    [
        UserPermission.BackOfficeBotList,
        UserPermission.BackOfficeBotStart
    ];

    protected override string CommandInternal => Command;

    protected override Task<bool> PerformCommandAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return botStartBo.StartAsync(
            context.Base.Bot,
            context,
            cancellationToken);
    }
}