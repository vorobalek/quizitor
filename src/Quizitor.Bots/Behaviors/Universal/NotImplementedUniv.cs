using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;

namespace Quizitor.Bots.Behaviors.Universal;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IBehaviorContext>;
using Context = ICallbackQueryDataEqualsContext<IBehaviorContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class NotImplementedUniv :
    UniversalBehavior,
    Behavior
{
    public const string Button = "notimplemented";

    public override string[] Permissions => [];
    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .AnswerCallbackQuery(
                context.Base.UpdateContext,
                context.CallbackQueryId,
                TR.L + "_SHARED_NOT_IMPLEMENTED_TXT",
                true,
                cancellationToken: cancellationToken);
    }
}