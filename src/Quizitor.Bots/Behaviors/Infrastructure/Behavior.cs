using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryData;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.Behaviors.Infrastructure.MessageText;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.Behaviors.Infrastructure.QrCodeDataPrefix;
using Quizitor.Bots.Exceptions;
using Quizitor.Bots.Services.Identity;
using Quizitor.Bots.UI.Shared;
using Quizitor.Data.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.Infrastructure;

internal abstract partial class Behavior<TContext> :
    IBehavior
    where TContext : IBehaviorContext
{
    public abstract BotType Type { get; }
    public abstract string[] Permissions { get; }

    public bool ShouldPerform(IBehaviorContext baseContext)
    {
        return ShouldPerformTraits(baseContext) || ShouldPerformInternal(baseContext);
    }

    public async Task PerformAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        try
        {
            AssertPermissions(baseContext.Identity);

            await PerformSpecificAsync(
                baseContext,
                cancellationToken);

            if (ShouldPerformInternal(baseContext) &&
                await PrepareAsync(
                    baseContext,
                    cancellationToken) is { } context)
                await PerformInternalAsync(
                    context,
                    cancellationToken);
        }
        catch (UnauthorizedException exception)
        {
            await HandleUnauthorizedAsync(
                exception,
                baseContext,
                cancellationToken);
        }
    }

    private void AssertPermissions(IIdentity identity)
    {
        AssertPermissions(identity, Permissions);
    }

    private static void AssertPermissions(
        IIdentity identity,
        params string[] permissions)
    {
        if (identity.HasFullAccess) return;

        var missingPermissions = permissions
            .Except(
                identity.Permissions,
                StringComparer.InvariantCultureIgnoreCase)
            .ToArray();

        if (missingPermissions.Length != 0)
            throw new UnauthorizedException(missingPermissions);
    }

    protected virtual bool ShouldPerformInternal(IBehaviorContext baseContext)
    {
        return false;
    }

    protected Task<TContext?> PrepareAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return PrepareInternalAsync(
            baseContext,
            cancellationToken);
    }

    protected abstract Task<TContext?> PrepareInternalAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken);

    protected virtual Task PerformInternalAsync(
        TContext context,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task HandleUnauthorizedAsync(
        UnauthorizedException exception,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        return HandleUnauthorizedInternalAsync(
            exception,
            baseContext,
            cancellationToken);
    }

    protected virtual Task HandleUnauthorizedInternalAsync(
        UnauthorizedException exception,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        var missingPermissions = string.Join(
            ", ",
            exception.MissingPermissions);

        var missingPermissionsList = string.Join(
            ";\n",
            exception.MissingPermissions
                .Select(permission => string.Format(
                    TR.L + "_SHARED_PERMISSION_LIST_ITEM_TXT",
                    TR.L + $"_SHARED_PERMISSION_{permission}",
                    permission)));

        return baseContext.UpdateContext.Update switch
        {
            {
                Type: UpdateType.Message,
                Message.Id: var messageId
            } => baseContext
                .Client
                .SendMessage(
                    baseContext.UpdateContext,
                    baseContext.TelegramUser.Id,
                    string.Format(
                        TR.L + "_SHARED_UNAUTHORIZED_TXT",
                        missingPermissionsList),
                    ParseMode.Html,
                    new ReplyParameters
                    {
                        MessageId = messageId
                    }, cancellationToken: cancellationToken),
            {
                Type: UpdateType.CallbackQuery,
                CallbackQuery.Id: var callbackQueryId
            } => baseContext
                .Client
                .AnswerCallbackQuery(
                    baseContext.UpdateContext,
                    callbackQueryId,
                    string.Format(
                        TR.L + "_SHARED_UNAUTHORIZED_CLB",
                        missingPermissions),
                    true,
                    cancellationToken: cancellationToken),
            _ => Task.CompletedTask
        };
    }

    private bool ShouldPerformTraits(IBehaviorContext baseContext)
    {
        var result = false;

        result |= ShouldPerformSpecificTrait<
            ICallbackQueryDataEqualsBehaviorTrait<TContext>,
            ICallbackQueryDataEqualsContext<TContext>
        >(
            this,
            baseContext);

        result |= ShouldPerformSpecificTrait<
            ICallbackQueryDataPrefixBehaviorTrait<TContext>,
            ICallbackQueryDataPrefixContext<TContext>
        >(
            this,
            baseContext);

        result |= ShouldPerformSpecificTrait<
            IMessageTextBehaviorTrait<TContext>,
            IMessageTextContext<TContext>
        >(
            this,
            baseContext);

        result |= ShouldPerformSpecificTrait<
            IMessageTextBotCommandEqualsBehaviorTrait<TContext>,
            IMessageTextBotCommandEqualsContext<TContext>
        >(
            this,
            baseContext);

        result |= ShouldPerformSpecificTrait<
            IQrCodeDataPrefixBehaviorTrait<TContext>,
            IQrCodeDataPrefixContext<TContext>
        >(
            this,
            baseContext);

        return result;
    }

    private static bool ShouldPerformSpecificTrait<TTrait, TTraitContext>(
        IBehavior behavior,
        IBehaviorContext baseContext)
        where TTrait : IBehaviorTrait<TTraitContext>
        where TTraitContext : IBehaviorTraitContext<TContext>
    {
        return behavior is TTrait trait &&
               trait.ShouldPerformSpecific(baseContext);
    }

    private async Task PerformSpecificAsync(
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
    {
        await PerformSpecificTraitAsync<
            ICallbackQueryDataEqualsBehaviorTrait<TContext>,
            ICallbackQueryDataEqualsContext<TContext>
        >(
            this,
            baseContext,
            cancellationToken);

        await PerformSpecificTraitAsync<
            ICallbackQueryDataPrefixBehaviorTrait<TContext>,
            ICallbackQueryDataPrefixContext<TContext>
        >(
            this,
            baseContext,
            cancellationToken);

        await PerformSpecificTraitAsync<
            IMessageTextBehaviorTrait<TContext>,
            IMessageTextContext<TContext>
        >(
            this,
            baseContext,
            cancellationToken);

        await PerformSpecificTraitAsync<
            IMessageTextBotCommandEqualsBehaviorTrait<TContext>,
            IMessageTextBotCommandEqualsContext<TContext>
        >(
            this,
            baseContext,
            cancellationToken);

        await PerformSpecificTraitAsync<
            IQrCodeDataPrefixBehaviorTrait<TContext>,
            IQrCodeDataPrefixContext<TContext>
        >(
            this,
            baseContext,
            cancellationToken);
    }

    private static async Task PerformSpecificTraitAsync<TTrait, TTraitContext>(
        IBehavior behavior,
        IBehaviorContext baseContext,
        CancellationToken cancellationToken)
        where TTrait : IBehaviorTrait<TTraitContext>
        where TTraitContext : IBehaviorTraitContext<TContext>
    {
        if (behavior is TTrait trait &&
            trait.ShouldPerformSpecific(baseContext) &&
            await trait.PrepareSpecificAsync(
                baseContext,
                cancellationToken) is { } context)
            await trait.PerformSpecificAsync(context, cancellationToken);
    }

    protected static async Task FallbackWrongUserPromptAsync<TCallbackQueryContext>(
        TCallbackQueryContext context,
        UserPromptType userPromptType,
        CancellationToken cancellationToken)
        where TCallbackQueryContext : ICallbackQueryDataContext<IBehaviorContext>
    {
        await context
            .Base
            .Client
            .AnswerCallbackQuery(
                context.Base.UpdateContext,
                context.CallbackQueryId,
                string
                    .Format(
                        TR.L + "_SHARED_PROMPT_WRONG_CLB",
                        TR.L + $"_SHARED_PROMPT_{userPromptType}_CLB"),
                true,
                cancellationToken: cancellationToken
            );

        await context
            .Base
            .Client
            .SendMessage(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                TR.L + $"_SHARED_PROMPT_MESSAGE_{userPromptType}_TXT",
                ParseMode.Html,
                replyMarkup: Keyboards.CancelPrompt,
                cancellationToken: cancellationToken);
    }
}