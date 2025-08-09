using LPlus;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Infrastructure;
using Quizitor.Bots.Behaviors.Infrastructure.CallbackQueryDataPrefix;
using Quizitor.Bots.UI.BackOffice;
using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.Behaviors.BackOffice.Mailings;

// ReSharper disable once ClassNeverInstantiated.Global
internal abstract class MailingProfileBo<TContext>(
    IDbContextProvider dbContextProvider) :
    BackOfficeBehavior<TContext>,
    ICallbackQueryDataPrefixBehaviorTrait<TContext>
    where TContext : IMailingProfileBackOfficeContext
{
    protected abstract string ButtonInline { get; }

    public override string[] Permissions => [UserPermission.BackOfficeMailingView];

    public string CallbackQueryDataPrefixValue => $"{ButtonInline}.";

    public virtual Task PerformCallbackQueryDataPrefixAsync(
        ICallbackQueryDataPrefixContext<TContext> context,
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
                    TR.L + "_BACKOFFICE_MAILING_PROFILE_TXT",
                    context.Base.Mailing.Name.EscapeHtml(),
                    context.Base.PredictedMessagesCount,
                    context.Base.PredictedUsersCount,
                    context.Base.PredictedBotsCount,
                    GetGameIncludedFilterText(context.Base),
                    GetGameExcludedFilterText(context.Base),
                    GetSessionIncludedFilterText(context.Base),
                    GetSessionExcludedFilterText(context.Base),
                    GetTeamIncludedFilterText(context.Base),
                    GetTeamExcludedFilterText(context.Base),
                    GetUserIncludedFilterText(context.Base),
                    GetUserExcludedFilterText(context.Base),
                    TR.L + $"_BACKOFFICE_MAILING_PROFILE_CONTACT_{context.Base.ContactType}_TXT",
                    GetBotTypeIncludedFilterText(context.Base),
                    GetBotTypeExcludedFilterText(context.Base),
                    GetBotIncludedFilterText(context.Base),
                    GetBotExcludedFilterText(context.Base)),
                ParseMode.Html,
                replyMarkup: KeyboardMarkup(context.Base),
                cancellationToken: cancellationToken);
    }

    protected abstract InlineKeyboardMarkup KeyboardMarkup(TContext context);

    protected async Task<IMailingProfileBackOfficeContext> PrepareMailingProfileContextAsync(
        Mailing mailing,
        MailingProfile? mailingProfile,
        IBackOfficeContext context,
        int mailingPageNumber,
        CancellationToken cancellationToken)
    {
        var gamesIncluded = mailingProfile is not null
            ? await dbContextProvider
                .Games
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Include)
            : [];

        var gamesExcluded = mailingProfile is not null
            ? await dbContextProvider
                .Games
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Exclude)
            : [];

        var sessionsIncluded = mailingProfile is not null
            ? await dbContextProvider
                .Sessions
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Include)
            : [];

        var sessionsExcluded = mailingProfile is not null
            ? await dbContextProvider
                .Sessions
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Exclude)
            : [];

        var teamsIncluded = mailingProfile is not null
            ? await dbContextProvider
                .Teams
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Include)
            : [];

        var teamsExcluded = mailingProfile is not null
            ? await dbContextProvider
                .Teams
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Exclude)
            : [];

        var usersIncluded = mailingProfile is not null
            ? await dbContextProvider
                .Users
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Include)
            : [];

        var usersExcluded = mailingProfile is not null
            ? await dbContextProvider
                .Users
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Exclude)
            : [];

        var botsIncluded = mailingProfile is not null
            ? await dbContextProvider
                .Bots
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Include)
            : [];

        var botsExcluded = mailingProfile is not null
            ? await dbContextProvider
                .Bots
                .GetByMailingProfileIdAsync(
                    mailingProfile.Id,
                    cancellationToken,
                    MailingFilterFlagType.Exclude)
            : [];

        var recipients = await dbContextProvider
            .Mailings
            .GetRecipientsAsync(
                gamesIncluded.Select(x => x.Id).ToArray(),
                gamesExcluded.Select(x => x.Id).ToArray(),
                sessionsIncluded.Select(x => x.Id).ToArray(),
                sessionsExcluded.Select(x => x.Id).ToArray(),
                teamsIncluded.Select(x => x.Id).ToArray(),
                teamsExcluded.Select(x => x.Id).ToArray(),
                usersIncluded.Select(x => x.Id).ToArray(),
                usersExcluded.Select(x => x.Id).ToArray(),
                cancellationToken);

        var includedBotTypes = mailingProfile is not null
            ? mailingProfile
                .BotTypes
                .Where(x => x.Value == MailingFilterFlagType.Include)
                .Select(x => x.Key)
                .ToArray()
            : [];

        var excludedBotTypes = mailingProfile is not null
            ? mailingProfile
                .BotTypes
                .Where(x => x.Value == MailingFilterFlagType.Exclude)
                .Select(x => x.Key)
                .ToArray()
            : [];

        var contactType = mailingProfile?.ContactType ?? MailingProfileContactType.All;

        var userIds = recipients.Select(x => x.Id).Distinct().ToArray();

        var schema = await dbContextProvider
            .Mailings
            .GetMailingSchemaForUsersAsync(
                contactType,
                includedBotTypes,
                excludedBotTypes,
                botsIncluded.Select(x => x.Id).ToArray(),
                botsExcluded.Select(x => x.Id).ToArray(),
                userIds,
                cancellationToken);

        var predictedUsersCount = schema.Where(x => x.Any()).Select(x => x.Key.Id).Count();
        var predictedMessagesCount = schema.Sum(x => x.Count());
        var predictedBotsCount = schema.SelectMany(x => x).DistinctBy(x => x.Id).Count();

        return IMailingProfileBackOfficeContext.Create(
            mailing,
            mailingPageNumber,
            gamesIncluded,
            gamesExcluded,
            sessionsIncluded,
            sessionsExcluded,
            teamsIncluded,
            teamsExcluded,
            usersIncluded,
            usersExcluded,
            botsIncluded,
            botsExcluded,
            includedBotTypes,
            excludedBotTypes,
            contactType,
            predictedUsersCount,
            predictedMessagesCount,
            predictedBotsCount,
            schema,
            context);
    }

    private static string GetFilterText<TEntity>(
        TEntity[] array,
        Func<TEntity, object?[]> itemArgsGetter,
        string allText,
        string listMoreText,
        string listText,
        string itemTxt)
    {
        const int maxLines = 5;
        return array.Length == 0
            ? allText
            : string.Format(
                array.Length > maxLines
                    ? listMoreText
                    : listText,
                string.Join(
                    TR.L + "_BACKOFFICE_MAILING_PROFILE_SEPARATOR",
                    array.Take(maxLines).Select(x =>
                        string.Format(
                            itemTxt,
                            itemArgsGetter(x)))),
                array.Length - maxLines);
    }

    private static string GetGameFilterText(
        Game[] array,
        string allText)
    {
        return GetFilterText(
            array,
            x =>
            [
                x.Title.EscapeHtml()
            ],
            allText,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_GAMES_LIST_MORE_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_GAMES_LIST_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_GAMES_ITEM_TXT");
    }

    private static string GetGameIncludedFilterText(TContext context)
    {
        return GetGameFilterText(
            context.IncludedGames,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_GAMES_ALL_TXT");
    }

    private static string GetGameExcludedFilterText(TContext context)
    {
        return GetGameFilterText(
            context.ExcludedGames,
            TR.L + "_SHARED_NO_TXT");
    }

    private static string GetSessionFilterText(
        Session[] array,
        string allText)
    {
        return GetFilterText(
            array,
            x =>
            [
                x.Game.Title.EscapeHtml(),
                x.Name.EscapeHtml()
            ],
            allText,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_SESSIONS_LIST_MORE_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_SESSIONS_LIST_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_SESSIONS_ITEM_TXT");
    }

    private static string GetSessionIncludedFilterText(TContext context)
    {
        return GetSessionFilterText(
            context.IncludedSessions,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_SESSIONS_ALL_TXT");
    }

    private static string GetSessionExcludedFilterText(TContext context)
    {
        return GetSessionFilterText(
            context.ExcludedSessions,
            TR.L + "_SHARED_NO_TXT");
    }

    private static string GetTeamFilterText(
        Team[] array,
        string allText)
    {
        return GetFilterText(
            array,
            x =>
            [
                x.Name.EscapeHtml()
            ],
            allText,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_TEAMS_LIST_MORE_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_TEAMS_LIST_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_TEAMS_ITEM_TXT");
    }

    private static string GetTeamIncludedFilterText(TContext context)
    {
        return GetTeamFilterText(
            context.IncludedTeams,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_TEAMS_ALL_TXT");
    }

    private static string GetTeamExcludedFilterText(TContext context)
    {
        return GetTeamFilterText(
            context.ExcludedTeams,
            TR.L + "_SHARED_NO_TXT");
    }

    private static string GetUserFilterText(
        User[] array,
        string allText)
    {
        return GetFilterText(
            array,
            x =>
            [
                x.Id,
                x.GetFullName().EscapeHtml()
            ],
            allText,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_USERS_LIST_MORE_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_USERS_LIST_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_USERS_ITEM_TXT");
    }

    private static string GetUserIncludedFilterText(TContext context)
    {
        return GetUserFilterText(
            context.IncludedUsers,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_USERS_ALL_TXT");
    }

    private static string GetUserExcludedFilterText(TContext context)
    {
        return GetUserFilterText(
            context.ExcludedUsers,
            TR.L + "_SHARED_NO_TXT");
    }

    private static string GetBotTypeFilterText(
        BotType[] array,
        string allText)
    {
        return GetFilterText(
            array,
            x =>
            [
                x
            ],
            allText,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOT_TYPES_LIST_MORE_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOT_TYPES_LIST_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOT_TYPES_ITEM_TXT");
    }

    private static string GetBotTypeIncludedFilterText(TContext context)
    {
        return GetBotTypeFilterText(
            context.IncludedBotTypes,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOT_TYPES_ALL_TXT");
    }

    private static string GetBotTypeExcludedFilterText(TContext context)
    {
        return GetBotTypeFilterText(
            context.ExcludedBotTypes,
            TR.L + "_SHARED_NO_TXT");
    }

    private static string GetBotFilterText(
        Bot[] array,
        string allText)
    {
        return GetFilterText(
            array,
            x =>
            [
                (x.Username ?? x.Name).EscapeHtml()
            ],
            allText,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOTS_LIST_MORE_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOTS_LIST_TXT",
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOTS_ITEM_TXT");
    }

    private static string GetBotIncludedFilterText(TContext context)
    {
        return GetBotFilterText(
            context.IncludedBots,
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BOTS_ALL_TXT");
    }

    private static string GetBotExcludedFilterText(TContext context)
    {
        return GetBotFilterText(
            context.ExcludedBots,
            TR.L + "_SHARED_NO_TXT");
    }
}

internal class MailingProfileBo(IDbContextProvider dbContextProvider) :
    MailingProfileBo<IMailingProfileBackOfficeContext>(dbContextProvider)
{
    /// <summary>
    ///     <b>mailingprofile</b>.{mailingId}.{mailingPageNumber}
    /// </summary>
    public const string Button = "mailingprofile";

    private readonly IDbContextProvider _dbContextProvider = dbContextProvider;

    protected override string ButtonInline => Button;

    protected override InlineKeyboardMarkup KeyboardMarkup(IMailingProfileBackOfficeContext context)
    {
        return Keyboards.MailingProfile(
            context.Mailing.Id,
            context.MailingPageNumber);
    }

    protected override async Task<IMailingProfileBackOfficeContext?> PrepareContextAsync(
        IBackOfficeContext backOfficeContext,
        CancellationToken cancellationToken)
    {
        var callbackContext = ICallbackQueryDataPrefixContext.Create(backOfficeContext, CallbackQueryDataPrefixValue);
        if (callbackContext?.CallbackQueryDataPostfix.Split('.') is
            [
                {
                } mailingIdString,
                {
                } mailingPageNumberString
            ] &&
            int.TryParse(mailingIdString, out var mailingId) &&
            int.TryParse(mailingPageNumberString, out var mailingPageNumber) &&
            await _dbContextProvider.Mailings.GetByIdOrDefaultAsync(mailingId, cancellationToken) is { } mailing)
        {
            var mailingProfile = await _dbContextProvider
                .Mailings
                .GetProfileByMailingIdUserIdOrDefaultAsync(
                    mailing.Id,
                    backOfficeContext.Identity.User.Id,
                    cancellationToken);

            return await PrepareMailingProfileContextAsync(
                mailing,
                mailingProfile,
                backOfficeContext,
                mailingPageNumber,
                cancellationToken);
        }

        return null;
    }
}