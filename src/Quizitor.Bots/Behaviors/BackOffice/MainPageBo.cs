using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataEquals;
using Quizitor.Bots.Behaviors.Infrastructure.MessageTextBotCommandEquals;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Telegram.Bot.Types.Enums;

namespace Quizitor.Bots.Behaviors.BackOffice;

using MessageBehavior = IMessageTextBotCommandEqualsBehaviorTrait<IMainPageBackOfficeContext>;
using MessageContext = IMessageTextBotCommandEqualsContext<IMainPageBackOfficeContext>;
using CallbackQueryBehavior = ICallbackQueryDataEqualsBehaviorTrait<IMainPageBackOfficeContext>;
using CallbackQueryContext = ICallbackQueryDataEqualsContext<IMainPageBackOfficeContext>;

// ReSharper disable once ClassNeverInstantiated.Global
internal sealed class MainPageBo(IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<IMainPageBackOfficeContext>,
    MessageBehavior,
    CallbackQueryBehavior
{
    /// <summary>
    ///     <b>start</b><br />
    ///     /<b>start</b>
    /// </summary>
    public const string Button = "start";

    public override string[] Permissions => [UserPermission.BackOfficeMain];

    public string CallbackQueryDataValue => Button;

    public Task PerformCallbackQueryDataEqualsAsync(
        CallbackQueryContext context,
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
                    TR.L + "_BACKOFFICE_MAIN_TXT",
                    context.Base.TelegramUser.FirstName.EscapeHtml(),
                    context.Base.TelegramUser.LastName?.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.MainPage(
                    context.Base.BotsCount,
                    context.Base.UsersCount,
                    context.Base.MailingsCount,
                    context.Base.GamesCount),
                cancellationToken: cancellationToken);
    }

    public string BotCommandValue => Button;

    public Task PerformMessageTextBotCommandEqualsAsync(
        MessageContext context,
        CancellationToken cancellationToken)
    {
        return context
            .Base
            .Client
            .SendMessage(
                context.Base.UpdateContext,
                context.Base.TelegramUser.Id,
                string.Format(
                    TR.L + "_BACKOFFICE_MAIN_TXT",
                    context.Base.TelegramUser.FirstName.EscapeHtml(),
                    context.Base.TelegramUser.LastName?.EscapeHtml()),
                ParseMode.Html,
                replyMarkup: Keyboards.MainPage(
                    context.Base.BotsCount,
                    context.Base.UsersCount,
                    context.Base.MailingsCount,
                    context.Base.GamesCount),
                cancellationToken: cancellationToken);
    }

    protected override async Task<IMainPageBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var botsCount = await dbContextProvider.Bots.CountAsync(cancellationToken);
        var usersCount = await dbContextProvider.Users.CountAsync(cancellationToken);
        var mailingsCount = await dbContextProvider.Mailings.CountAsync(cancellationToken);
        var gamesCount = await dbContextProvider.Games.CountAsync(cancellationToken);

        return IMainPageBackOfficeContext.Create(
            botsCount,
            usersCount,
            mailingsCount,
            gamesCount,
            backOfficeContext);
    }
}