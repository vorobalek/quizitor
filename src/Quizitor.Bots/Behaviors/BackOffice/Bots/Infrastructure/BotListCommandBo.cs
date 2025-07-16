using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Data;

namespace Quizitor.Bots.Behaviors.BackOffice.Bots.Infrastructure;

using Context = ICallbackQueryDataPrefixContext<IBotListCommandBackOfficeContext>;

internal abstract class BotListCommandBo(
    IDbContextProvider dbContextProvider) :
    BotListBo<IBotListCommandBackOfficeContext>(
        dbContextProvider)
{
    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected abstract string CommandInternal { get; }

    protected override async Task<IBotListCommandBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } pageNumberString,
                {
                } command,
                {
                } botIdString
            ] &&
            command == CommandInternal &&
            int.TryParse(pageNumberString, out var pageNumber) &&
            int.TryParse(botIdString, out var botId) &&
            await _dbContextProvider.Bots.GetByIdOrDefaultAsync(botId, cancellationToken) is { } bot)
        {
            var botListContext = await PrepareBaseAsync(
                backOfficeContext,
                pageNumber,
                cancellationToken);
            return IBotListCommandBackOfficeContext.Create(
                bot,
                botListContext);
        }

        return null;
    }

    public override bool ShouldPerformCallbackQueryDataPrefix(IBehaviorContext baseContext)
    {
        return base.ShouldPerformCallbackQueryDataPrefix(baseContext) &&
               baseContext.UpdateContext.Update.CallbackQuery!.Data!.Split('.') is { Length: > 2 } args &&
               args[2] == CommandInternal;
    }

    public override async Task PerformCallbackQueryDataPrefixAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        if (await PerformCommandAsync(context, cancellationToken))
            _dbContextProvider
                .AddPostCommitTask(async () =>
                    await ResponseAsync(context, cancellationToken));
        else
            await ResponseAsync(context, cancellationToken);
    }

    protected abstract Task<bool> PerformCommandAsync(
        Context context,
        CancellationToken cancellationToken);
}