using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;

internal interface IMessageTextBotCommandEqualsContext : IMessageTextContext
{
    static bool IsValidUpdate(
        Update update,
        bool allowInline = false)
    {
        return
            update is
            {
                Type: UpdateType.Message,
                Message:
                {
                    Type: MessageType.Text,
                    Photo: null,
                    Entities:
                    {
                        Length: > 0
                    } entities
                }
            } &&
            entities.Any(x =>
                x.Type == MessageEntityType.BotCommand &&
                (allowInline || x.Offset == 0));
    }

    static bool IsValidUpdate(
        Update update,
        string botCommandValue,
        bool allowInline = false)
    {
        return
            update is
            {
                Type: UpdateType.Message,
                Message:
                {
                    Type: MessageType.Text,
                    Photo: null,
                    From: not null,
                    Text: not null and var messageText,
                    Entities:
                    {
                        Length: > 0
                    } entities
                }
            } &&
            entities.Any(x =>
                x.Type == MessageEntityType.BotCommand &&
                messageText.Length > x.Offset + 1 &&
                messageText.Length >= x.Offset + x.Length &&
                messageText[(x.Offset + 1) .. (x.Offset + x.Length)].Trim() == botCommandValue &&
                (allowInline || x.Offset == 0));
    }

    static IMessageTextBotCommandEqualsContext<TContext>? Create<TContext>(
        TContext baseContext,
        string botCommandValue)
        where TContext : IBehaviorContext
    {
        return IsValidUpdate(baseContext.UpdateContext.Update, botCommandValue)
            ? new MessageTextBotCommandEqualsContext<TContext>(
                baseContext,
                baseContext.UpdateContext.Update.Message!.From!.Id,
                baseContext.UpdateContext.Update.Message.Text!)
            : null;
    }

    private record MessageTextBotCommandEqualsContext<TContext>(
        TContext Base,
        long MessageFromId,
        string MessageText) : IMessageTextBotCommandEqualsContext<TContext>
        where TContext : IBehaviorContext;
}

internal interface IMessageTextBotCommandEqualsContext<out TContext> :
    IBehaviorTraitContext<TContext>,
    IMessageTextBotCommandEqualsContext
    where TContext : IBehaviorContext;