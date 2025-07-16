using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryData;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;

internal interface ICallbackQueryDataEqualsContext : ICallbackQueryDataContext
{
    static bool IsValidUpdate(
        Update update,
        string callbackQueryDataValue)
    {
        return
            update is
            {
                Type: UpdateType.CallbackQuery,
                CallbackQuery:
                {
                    Data: { } callbackQueryData,
                    Message:
                    {
                        Type: MessageType.Text,
                        Text: not null
                    }
                }
            } &&
            callbackQueryData == callbackQueryDataValue;
    }

    static ICallbackQueryDataEqualsContext<TContext>? Create<TContext>(
        TContext baseContext,
        string callbackQueryDataValue)
        where TContext : IBehaviorContext
    {
        return IsValidUpdate(baseContext.UpdateContext.Update, callbackQueryDataValue)
            ? new CallbackQueryDataEqualsContext<TContext>(
                baseContext.UpdateContext.Update.CallbackQuery!.Id,
                baseContext.UpdateContext.Update.CallbackQuery.From.Id,
                baseContext.UpdateContext.Update.CallbackQuery.Message!.Id,
                baseContext.UpdateContext.Update.CallbackQuery.Message.Text!,
                baseContext.UpdateContext.Update.CallbackQuery.Message.Entities,
                baseContext)
            : null;
    }

    private record CallbackQueryDataEqualsContext<TContext>(
        string CallbackQueryId,
        long CallbackQueryFromId,
        int MessageId,
        string MessageText,
        MessageEntity[]? MessageEntities,
        TContext Base) : ICallbackQueryDataEqualsContext<TContext>
        where TContext : IBehaviorContext;
}

internal interface ICallbackQueryDataEqualsContext<out TContext> :
    ICallbackQueryDataContext<TContext>,
    ICallbackQueryDataEqualsContext
    where TContext : IBehaviorContext;