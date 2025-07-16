using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryData;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;

internal interface ICallbackQueryDataPrefixContext : ICallbackQueryDataContext
{
    string CallbackQueryDataPostfix { get; }

    static bool IsValidUpdate(
        Update update,
        string callbackQueryDataPrefixValue)
    {
        return
            update is
            {
                Type: UpdateType.CallbackQuery,
                CallbackQuery:
                {
                    Data: { } callbackQueryData,
                    Message: not null
                }
            } &&
            callbackQueryData.StartsWith(callbackQueryDataPrefixValue);
    }

    static ICallbackQueryDataPrefixContext<TContext>? Create<TContext>(
        TContext baseContext,
        string callbackQueryDataPrefixValue)
        where TContext : IBehaviorContext
    {
        return IsValidUpdate(baseContext.UpdateContext.Update, callbackQueryDataPrefixValue)
            ? new CallbackQueryDataPrefixContext<TContext>(
                baseContext.UpdateContext.Update.CallbackQuery!.From.Id,
                baseContext.UpdateContext.Update.CallbackQuery.Data![callbackQueryDataPrefixValue.Length ..],
                baseContext.UpdateContext.Update.CallbackQuery.Id,
                baseContext.UpdateContext.Update.CallbackQuery.Message!.Id,
                baseContext.UpdateContext.Update.CallbackQuery.Message.Text!,
                baseContext.UpdateContext.Update.CallbackQuery.Message.Entities,
                baseContext)
            : null;
    }

    private record CallbackQueryDataPrefixContext<TContext>(
        long CallbackQueryFromId,
        string CallbackQueryDataPostfix,
        string CallbackQueryId,
        int MessageId,
        string MessageText,
        MessageEntity[]? MessageEntities,
        TContext Base) : ICallbackQueryDataPrefixContext<TContext>
        where TContext : IBehaviorContext;
}

internal interface ICallbackQueryDataPrefixContext<out TContext> :
    ICallbackQueryDataContext<TContext>,
    ICallbackQueryDataPrefixContext
    where TContext : IBehaviorContext;