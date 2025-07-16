using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Data;
using Quizitor.Data.Entities;

namespace Quizitor.Bots.Behaviors.BackOffice.Services;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IBackOfficeContext>;
using Context = ICallbackQueryDataEqualsContext<IBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class UnlinkUserSessionsBo(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior,
    Behavior
{
    public const string Button = "unlinkusersessions";

    public override string[] Permissions => [UserPermission.BackOfficeUnlinkUserSessions];

    public string CallbackQueryDataValue => Button;

    public async Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        var users = await dbContextProvider
            .Users
            .GetAllAsync(cancellationToken);

        foreach (var user in users)
        {
            user.SessionId = null;
            await dbContextProvider
                .Users
                .UpdateAsync(
                    user,
                    cancellationToken);
        }

        dbContextProvider.AddPostCommitTask(async () =>
            await context
                .Base
                .Client
                .AnswerCallbackQuery(
                    context.Base.UpdateContext,
                    context.CallbackQueryId,
                    "Done!",
                    true,
                    cancellationToken: cancellationToken));
    }
}