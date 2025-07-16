using LPlus;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice.Services;

using Behavior = ICallbackQueryDataEqualsBehaviorTrait<IBackOfficeContext>;
using Context = ICallbackQueryDataEqualsContext<IBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class ServicePageBo :
    BackOfficeBehavior,
    Behavior
{
    public const string Button = "service";

    public override string[] Permissions => [UserPermission.BackOfficeServiceView];

    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        Context context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .EditMessageText(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                context.MessageId,
                string.Format(
                    TR.L + "_BACKOFFICE_SERVICE_TXT",
                    context.Base.TelegramUser.FirstName.EscapeHtml(),
                    context.Base.TelegramUser.LastName?.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.ServicePage, cancellationToken: cancellationToken);
    }
}