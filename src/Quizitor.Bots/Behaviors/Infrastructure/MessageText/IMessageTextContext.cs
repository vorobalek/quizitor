using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure.MessageText;

internal interface IMessageTextContext
{
    long MessageFromId { get; }
    string MessageText { get; }

    static bool IsValidUpdate(Update update)
    {
        return
            update is
            {
                Type: UpdateType.Message,
                Message:
                {
                    Type: MessageType.Text,
                    From: not null,
                    Text: not null,
                    Photo: null
                }
            };
    }

    static IMessageTextContext<TContext>? Create<TContext>(TContext baseContext)
        where TContext : IBehaviorContext
    {
        return IsValidUpdate(baseContext.UpdateContext.Update)
            ? new MessageTextContext<TContext>(
                baseContext,
                baseContext.UpdateContext.Update.Message!.From!.Id,
                baseContext.UpdateContext.Update.Message.Text!)
            : null;
    }

    private record MessageTextContext<TContext>(
        TContext Base,
        long MessageFromId,
        string MessageText) : IMessageTextContext<TContext>
        where TContext : IBehaviorContext;
}

internal interface IMessageTextContext<out TContext> :
    IBehaviorTraitContext<TContext>,
    IMessageTextContext
    where TContext : IBehaviorContext;