using LPlus;
using Quizitor.Bots.Behaviors.BackOffice;
using Quizitor.Bots.Behaviors.BackOffice.Bots;
using Quizitor.Bots.Behaviors.BackOffice.Games;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds.Questions;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;
using Quizitor.Bots.Behaviors.BackOffice.Mailings;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Audience;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Channel;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Bots;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.BotTypes;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Games;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Sessions;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Teams;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Users;
using Quizitor.Bots.Behaviors.BackOffice.Services;
using Quizitor.Bots.Behaviors.BackOffice.Users;
using Quizitor.Bots.Behaviors.Universal;
using Quizitor.Common;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using MailingChannelBotListBo = Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Bots.MailingChannelBotListBo;

namespace Quizitor.Bots.UI.BackOffice;

internal static class Buttons
{
    public static readonly InlineKeyboardButton Load100 =
        InlineKeyboardButton.WithCallbackData(
            "LOAD 100",
            Load100Bo.Button);

    public static InlineKeyboardButton MainPage =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAIN_BTN",
            MainPageBo.Button);

    public static InlineKeyboardButton UserPermissions =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_USER_PERMISSIONS_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton EditGame =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_EDIT_GAME_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton EditRound =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_EDIT_ROUND_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton DeleteRound =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_DELETE_ROUND_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton CreateQuestion =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_CREATE_QUESTION_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton EditQuestion =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_EDIT_QUESTION_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton DeleteQuestion =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_DELETE_QUESTION_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton Service =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_SERVICE_BTN",
            ServicePageBo.Button);

    public static InlineKeyboardButton UnlinkSessions =>
        InlineKeyboardButton.WithCallbackData(
            "\u26a0\ufe0f UNLINK SESSIONS",
            UnlinkUserSessionsBo.Button);

    public static InlineKeyboardButton EditSession =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_EDIT_SESSION_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton DeleteSession =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_DELETE_SESSION_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton EditMailing =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_EDIT_MAILING_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton DeleteMailing =>
        InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_DELETE_MAILING_BTN",
            NotImplementedUniv.Button);

    public static InlineKeyboardButton BotList(int count)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_BOTS_BTN",
                count),
            $"{BotListBo.Button}.0");
    }

    public static InlineKeyboardButton UserList(int count)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_USERS_BTN",
                count),
            $"{UserListBo.Button}.0");
    }

    public static InlineKeyboardButton GameList(int count)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_GAMES_BTN",
                count),
            $"{GameListBo.Button}.0");
    }

    public static InlineKeyboardButton MailingList(int count)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_MAILINGS_BTN",
                count),
            $"{MailingListBo.Button}.0");
    }

    public static InlineKeyboardButton Bot(
        int botId,
        string botName,
        string? botTypeName,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_BOT_VIEW_BTN",
                botName,
                botTypeName),
            $"{BotViewBo.Button}.{botId}.{botPageNumber}");
    }

    public static InlineKeyboardButton BotViewStart(
        int botId,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_BOT_ACTIVATE_TXT",
            $"{BotStartBo.Button}.{botId}.{botPageNumber}");
    }

    public static InlineKeyboardButton BotListStart(
        int botId,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_BOT_ACTIVATE_TXT",
            $"{BotListBo.Button}.{botPageNumber}.{BotListStartBo.Command}.{botId}");
    }

    public static InlineKeyboardButton BotViewStop(
        int botId,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_BOT_INACTIVATE_TXT",
            $"{BotStopBo.Button}.{botId}.{botPageNumber}");
    }

    public static InlineKeyboardButton BotListStop(
        int botId,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_BOT_INACTIVATE_TXT",
            $"{BotListBo.Button}.{botPageNumber}.{BotListStopBo.Command}.{botId}");
    }

    public static InlineKeyboardButton BotType(
        int botId,
        BotType botType,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_BOT_SWITCH_TYPE_BTN",
                botType,
                Enum.GetName(
                    (BotType)((Convert.ToInt32(botType) + 1) % (Enum.GetValues<BotType>().Length - 1)))
            ),
            $"{BotTypeBo.Button}.{botId}.{botPageNumber}");
    }

    public static InlineKeyboardButton BotPending(
        int botId,
        bool dropPendingUpdates,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_BOT_SWITCH_DROP_PENDING_BTN",
                dropPendingUpdates
                    ? TR.L + "_SHARED_YES_TXT"
                    : TR.L + "_SHARED_NO_TXT",
                !dropPendingUpdates
                    ? TR.L + "_SHARED_YES_TXT"
                    : TR.L + "_SHARED_NO_TXT"
            ),
            $"{BotPendingBo.Button}.{botId}.{botPageNumber}");
    }

    public static InlineKeyboardButton BackToBotList(int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{BotListBo.Button}.{botPageNumber}");
    }

    public static InlineKeyboardButton User(
        long userId,
        string firstName,
        string? lastName,
        int userPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_USER_VIEW_BTN",
                userId,
                firstName,
                lastName),
            $"{UserViewBo.Button}.{userId}.{userPageNumber}");
    }

    public static InlineKeyboardButton BackToUserList(
        int userPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{UserListBo.Button}.{userPageNumber}");
    }

    public static InlineKeyboardButton UserRoles(
        long userId,
        int userPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_USER_ROLES_BTN",
            $"{UserRoleListBo.Button}.{userId}.{userPageNumber}.0");
    }

    public static InlineKeyboardButton UserRoleGrant(
        long userId,
        int roleId,
        string systemName,
        int userPageNumber,
        int userRolesPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_USER_ROLES_VIEW_DISABLED_BTN",
                systemName),
            $"{UserRoleListBo.Button}.{userId}.{userPageNumber}.{userRolesPageNumber}.{UserRoleListGrantBo.Command}.{roleId}");
    }

    public static InlineKeyboardButton UserRoleRevoke(
        long userId,
        int roleId,
        string systemName,
        int userPageNumber,
        int userRolesPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_USER_ROLES_VIEW_ENABLED_BTN",
                systemName),
            $"{UserRoleListBo.Button}.{userId}.{userPageNumber}.{userRolesPageNumber}.{UserRoleListRevokeBo.Command}.{roleId}");
    }

    public static InlineKeyboardButton BackToUser(
        long userId,
        int userPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{UserViewBo.Button}.{userId}.{userPageNumber}");
    }

    public static InlineKeyboardButton CreateGame(int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_CREATE_GAME_BTN",
            $"{CreateGameBo.Button}.{gamePageNumber}");
    }

    public static InlineKeyboardButton DeleteGame(
        int gameId,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_DELETE_GAME_BTN",
            $"{DeleteGameBo.Button}.{gameId}.{gamePageNumber}");
    }

    public static InlineKeyboardButton DeleteGameFakeConfirmation(
        int fakeId,
        int gameId,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + $"_BACKOFFICE_FAKE_CONFIRMATION_{fakeId}_BTN",
            $"{GameViewBo.Button}.{gameId}.{gamePageNumber}");
    }

    public static InlineKeyboardButton DeleteGameConfirmation(
        int gameId,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_DELETE_GAME_CONFIRMATION_BTN",
            $"{DeleteGameConfirmBo.Button}.{gameId}.{gamePageNumber}");
    }

    public static InlineKeyboardButton Game(
        int gameId,
        string title,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_GAME_VIEW_BTN",
                title),
            $"{GameViewBo.Button}.{gameId}.{gamePageNumber}");
    }

    public static InlineKeyboardButton RoundList(
        int count,
        int gameId,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_ROUND_LIST_BTN",
                count),
            $"{RoundListBo.Button}.{gameId}.{gamePageNumber}.0");
    }

    public static InlineKeyboardButton CreateRound(
        int gameId,
        int gamePageNumber,
        int roundPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_CREATE_ROUND_BTN",
            $"{CreateRoundBo.Button}.{gameId}.{gamePageNumber}.{roundPageNumber}");
    }

    public static InlineKeyboardButton Round(
        int roundId,
        int number,
        string title,
        int gamePageNumber,
        int roundPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_ROUND_VIEW_BTN",
                number,
                title),
            $"{RoundViewBo.Button}.{roundId}.{gamePageNumber}.{roundPageNumber}.0");
    }

    public static InlineKeyboardButton BackToGame(
        int gameId,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{GameViewBo.Button}.{gameId}.{gamePageNumber}");
    }

    public static InlineKeyboardButton Question(
        int questionId,
        int number,
        string title,
        int gamePageNumber,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_QUESTION_VIEW_BTN",
                number,
                title),
            $"{QuestionViewBo.Button}.{questionId}.{gamePageNumber}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton BackToRoundList(
        int gameId,
        int gamePageNumber,
        int roundPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{RoundListBo.Button}.{gameId}.{gamePageNumber}.{roundPageNumber}");
    }

    public static InlineKeyboardButton BackToRound(
        int roundId,
        int gamePageNumber,
        int roundPageNumber,
        int questionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{RoundViewBo.Button}.{roundId}.{gamePageNumber}.{roundPageNumber}.{questionPageNumber}");
    }

    public static InlineKeyboardButton SessionList(
        int count,
        int gameId,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_SESSION_LIST_BTN",
                count),
            $"{SessionListBo.Button}.{gameId}.{gamePageNumber}.0");
    }

    public static InlineKeyboardButton CreateSession(
        int gameId,
        int gamePageNumber,
        int sessionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_CREATE_SESSION_BTN",
            $"{CreateSessionBo.Button}.{gameId}.{gamePageNumber}.{sessionPageNumber}");
    }

    public static InlineKeyboardButton Session(
        int sessionId,
        string name,
        int gamePageNumber,
        int sessionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_SESSION_VIEW_BTN",
                name),
            $"{SessionViewBo.Button}.{sessionId}.{gamePageNumber}.{sessionPageNumber}");
    }

    public static InlineKeyboardButton SessionGetQr(
        int sessionId,
        int gamePageNumber,
        int sessionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_SESSION_GET_QR_BTN",
            $"{SessionGetQrBo.Button}.{sessionId}.{gamePageNumber}.{sessionPageNumber}");
    }

    public static InlineKeyboardButton BackToSessionList(
        int gameId,
        int gamePageNumber,
        int sessionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{SessionListBo.Button}.{gameId}.{gamePageNumber}.{sessionPageNumber}");
    }

    public static InlineKeyboardButton BackToGameList(
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{GameListBo.Button}.{gamePageNumber}");
    }

    public static InlineKeyboardButton CreateMailing(int pageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_CREATE_MAILING_BTN",
            $"{CreateMailingBo.Button}.{pageNumber}");
    }

    public static InlineKeyboardButton Mailing(
        int mailingId,
        string name,
        int pageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + "_BACKOFFICE_MAILING_VIEW_BTN",
                name),
            $"{MailingViewBo.Button}.{mailingId}.{pageNumber}");
    }

    public static InlineKeyboardButton MailingPreviewFormatting(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_PREVIEW_MAILING_BTN",
            $"{MailingPreviewBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingProfile(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_PROFILE_BTN",
            $"{MailingProfileBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingAudience(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_AUDIENCE_BTN",
            $"{MailingAudienceBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceGames(
        int mailingId,
        int mailingPageNumber,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_AUDIENCE_GAMES_BTN",
            $"{MailingAudienceGameListBo.Button}.{mailingId}.{mailingPageNumber}.{gamePageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceGame(
        Game game,
        MailingFilterFlagType flag,
        int mailingId,
        int mailingPageNumber,
        int gamePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + $"_BACKOFFICE_MAILING_AUDIENCE_GAME_{flag}_BTN",
                game.Title),
            $"{MailingAudienceGameBo.Button}.{mailingId}.{mailingPageNumber}.{game.Id}.{gamePageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceSessions(
        int mailingId,
        int mailingPageNumber,
        int sessionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_AUDIENCE_SESSIONS_BTN",
            $"{MailingAudienceSessionListBo.Button}.{mailingId}.{mailingPageNumber}.{sessionPageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceSession(
        Session session,
        MailingFilterFlagType flag,
        int mailingId,
        int mailingPageNumber,
        int sessionPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + $"_BACKOFFICE_MAILING_AUDIENCE_SESSION_{flag}_BTN",
                session.Game.Title,
                session.Name),
            $"{MailingAudienceSessionBo.Button}.{mailingId}.{mailingPageNumber}.{session.Id}.{sessionPageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceTeams(
        int mailingId,
        int mailingPageNumber,
        int teamPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_AUDIENCE_TEAMS_BTN",
            $"{MailingAudienceTeamListBo.Button}.{mailingId}.{mailingPageNumber}.{teamPageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceTeam(
        Team team,
        MailingFilterFlagType flag,
        int mailingId,
        int mailingPageNumber,
        int teamPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + $"_BACKOFFICE_MAILING_AUDIENCE_TEAM_{flag}_BTN",
                team.Name),
            $"{MailingAudienceTeamBo.Button}.{mailingId}.{mailingPageNumber}.{team.Id}.{teamPageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceUsers(
        int mailingId,
        int mailingPageNumber,
        int userPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_AUDIENCE_USERS_BTN",
            $"{MailingAudienceUserListBo.Button}.{mailingId}.{mailingPageNumber}.{userPageNumber}");
    }

    public static InlineKeyboardButton MailingAudienceUser(
        User user,
        MailingFilterFlagType flag,
        int mailingId,
        int mailingPageNumber,
        int userPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + $"_BACKOFFICE_MAILING_AUDIENCE_USER_{flag}_BTN",
                user.Id,
                user.GetFullName()),
            $"{MailingAudienceUserBo.Button}.{mailingId}.{mailingPageNumber}.{user.Id}.{userPageNumber}");
    }

    public static InlineKeyboardButton MailingChannel(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_CHANNEL_BTN",
            $"{MailingChannelBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingChannelBotType(
        BotType botType,
        MailingFilterFlagType flag,
        int mailingId,
        int mailingPageNumber,
        int botTypePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + $"_BACKOFFICE_MAILING_CHANNEL_BOT_TYPE_{flag}_BTN",
                botType,
                botType.GetDisplayName()),
            $"{MailingChannelBotTypeBo.Button}.{mailingId}.{mailingPageNumber}.{botType}.{botTypePageNumber}");
    }

    public static InlineKeyboardButton MailingChannelBot(
        Bot bot,
        MailingFilterFlagType flag,
        int mailingId,
        int mailingPageNumber,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            string.Format(
                TR.L + $"_BACKOFFICE_MAILING_CHANNEL_BOT_{flag}_BTN",
                bot.Username ?? bot.Name,
                bot.Type.GetDisplayName()),
            $"{MailingChannelBotBo.Button}.{mailingId}.{mailingPageNumber}.{bot.Id}.{botPageNumber}");
    }

    public static InlineKeyboardButton MailingSchema(
        int mailingId,
        int mailingPageNumber,
        int schemaPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_SCHEMA_BTN",
            $"{MailingSchemaBo.Button}.{mailingId}.{mailingPageNumber}.{schemaPageNumber}");
    }

    public static InlineKeyboardButton MailingChannelContactType(
        int mailingId,
        int mailingPageNumber,
        MailingProfileContactType contactType)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + $"_BACKOFFICE_MAILING_PROFILE_CONTACT_{contactType}_TXT",
            $"{MailingChannelContactTypeBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingChannelBotTypes(
        int mailingId,
        int mailingPageNumber,
        int botTypePageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_CHANNEL_BOT_TYPES_BTN",
            $"{MailingChannelBotTypeListBo.Button}.{mailingId}.{mailingPageNumber}.{botTypePageNumber}");
    }

    public static InlineKeyboardButton MailingChannelBots(
        int mailingId,
        int mailingPageNumber,
        int botPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_CHANNEL_BOTS_BTN",
            $"{MailingChannelBotListBo.Button}.{mailingId}.{mailingPageNumber}.{botPageNumber}");
    }

    public static InlineKeyboardButton MailingSend(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_SEND_BTN",
            $"{MailingSendBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingSendFakeConfirmation(
        int fakeId,
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + $"_BACKOFFICE_FAKE_CONFIRMATION_{fakeId}_BTN",
            $"{MailingProfileBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton MailingSendConfirmation(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_BACKOFFICE_MAILING_SEND_CONFIRMATION_BTN",
            $"{MailingSendConfirmBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton BackToMailingList(
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{MailingListBo.Button}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton BackToMailing(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{MailingViewBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton BackToMailingProfile(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{MailingProfileBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton BackToMailingAudience(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{MailingAudienceBo.Button}.{mailingId}.{mailingPageNumber}");
    }

    public static InlineKeyboardButton BackToMailingChannel(
        int mailingId,
        int mailingPageNumber)
    {
        return InlineKeyboardButton.WithCallbackData(
            TR.L + "_SHARED_BACK_BTN",
            $"{MailingChannelBo.Button}.{mailingId}.{mailingPageNumber}");
    }
}