using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Services;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IBackOfficeContext>;
using Context = ICallbackQueryDataEqualsContext<IBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class Load100Bo :
    BackOfficeBehavior,
    Behavior
{
    public const string Button = "load100";

    public override string[] Permissions => [UserPermission.BackOfficeMain];

    public string CallbackQueryDataValue => Button;

    public async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < 100; ++i)
            await context
                .Base
                .Client
                .SendMessage(
                    context.Base.UpdateContext,
                    context.Base.TelegramUser.Id,
                    $"Test message #{i + 1}",
                    cancellationToken: cancellationToken);
    }
}