using Telegram.Bot.Types;

namespace Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryData;

internal interface ICallbackQueryDataContext
{
    string CallbackQueryId { get; }
    long CallbackQueryFromId { get; }
    int MessageId { get; }
    string MessageText { get; }
    MessageEntity[]? MessageEntities { get; }
}

internal interface ICallbackQueryDataContext<out TBase> :
    IBehaviorTraitContext<TBase>,
    ICallbackQueryDataContext
    where TBase : IBehaviorContext;