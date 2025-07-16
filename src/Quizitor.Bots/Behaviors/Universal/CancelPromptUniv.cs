using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Data;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Universal;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IBehaviorContext>;
using Context = ICallbackQueryDataEqualsContext<IBehaviorContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class CancelPromptUniv(
    IDbContextProvider dbContextProvider) :
    UniversalBehavior,
    Behavior
{
    /// <summary>
    ///     <b>cancel</b>
    /// </summary>
    public const string Button = "cancel";

    public override string[] Permissions => [];
    public string CallbackQueryDataValue => Button;

    public async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        await context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                context.MessageText,
                entities: context.MessageEntities,
                replyMarkup: null,
                cancellationToken: cancellationToken);
        if (context.Base.Identity.Prompt?.Type is { } promptType)
        {
            if (promptType == UserPromptType.GameServerAnswer)
            {
                await context
                    .Base
                    .Client
                    .AnswerCallbackQuery(
                        context.Base.UpdateContext,
                        context.CallbackQueryId,
                        string.Format(
                            TR.L + "_SHARED_UNABLE_PROMPT_TO_CANCEL_CLB",
                            TR.L + $"_SHARED_PROMPT_{UserPromptType.GameServerAnswer}_CLB"),
                        true,
                        cancellationToken: cancellationToken);
                return;
            }

            await dbContextProvider
                .Users
                .RemovePromptByUserIdBotIdAsync(
                    context.Base.Identity.User.Id,
                    context.Base.EntryBot?.Id,
                    cancellationToken);
            dbContextProvider
                .AddPostCommitTask(async () =>
                    await context
                        .Base
                        .Client
                        .SendMessage(
                            context.Base.UpdateContext,
                            context.Base.TelegramUser.Id,
                            TR.L + $"_SHARED_PROMPT_CANCELLED_{promptType}_TXT",
                            ParseMode.Html,
                            cancellationToken: cancellationToken));
        }
        else
        {
            await context
                .Base
                .Client
                .AnswerCallbackQuery(
                    context.Base.UpdateContext,
                    context.CallbackQueryId,
                    TR.L + "_SHARED_NO_PROMPT_TO_CANCEL_CLB",
                    true,
                    cancellationToken: cancellationToken);
        }
    }
}