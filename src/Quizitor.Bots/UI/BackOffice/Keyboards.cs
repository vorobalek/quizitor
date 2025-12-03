using Quizitor.Bots.Behaviors.BackOffice.Bots;
using Quizitor.Bots.Behaviors.BackOffice.Games;
using Quizitor.Bots.Behaviors.BackOffice.Games.Rounds;
using Quizitor.Bots.Behaviors.BackOffice.Games.Sessions;
using Quizitor.Bots.Behaviors.BackOffice.Mailings;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Bots;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.BotTypes;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Games;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Sessions;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Teams;
using Quizitor.Bots.Behaviors.BackOffice.Mailings.Filters.Users;
using Quizitor.Bots.Behaviors.BackOffice.Users;
using Quizitor.Common;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Quizitor.Bots.UI.BackOffice;

internal static class Keyboards
{
    public static readonly InlineKeyboardMarkup ServicePage =
        new([[Buttons.UnlinkSessions, Buttons.Load100], [Buttons.MainPage]]);

    public static InlineKeyboardMarkup MainPage(
        int botsCount,
        int usersCount,
        int mailingsCount,
        int gamesCount)
    {
        return new InlineKeyboardMarkup([
            [Buttons.BotList(botsCount), Buttons.UserList(usersCount)],
            [Buttons.MailingList(mailingsCount)],
            [Buttons.GameList(gamesCount), Buttons.Service]
        ]);
    }

    public static InlineKeyboardMarkup BotList(
        IEnumerable<Bot> bots,
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            bots.Select(bot => new[]
                {
                    Buttons.Bot(
                        bot.Id,
                        bot.Name.Html,
                        bot.Type.DisplayName,
                        pageNumber),
                    bot.IsActive
                        ? Buttons.BotListStop(
                            bot.Id,
                            pageNumber)
                        : Buttons.BotListStart(
                            bot.Id,
                            pageNumber)
                })
                .Concat([
                    Shared.Buttons.BuildPaginationInlineButtons(
                        pageNumber,
                        pageCount,
                        number => $"{BotListBo.Button}.{number}",
                        number => $"{BotListBo.Button}.{number}")
                ])
                .Concat([[Buttons.MainPage]]));
    }

    public static InlineKeyboardMarkup BotView(
        Bot bot,
        int botPageNumber)
    {
        return new InlineKeyboardMarkup([
            [
                bot.IsActive
                    ? Buttons.BotViewStop(
                        bot.Id,
                        botPageNumber)
                    : Buttons.BotViewStart(
                        bot.Id,
                        botPageNumber)
            ],
            [
                Buttons.BotType(
                    bot.Id,
                    bot.Type,
                    botPageNumber)
            ],
            [
                Buttons.BotPending(
                    bot.Id,
                    bot.DropPendingUpdates,
                    botPageNumber)
            ],
            [
                Buttons.BackToBotList(botPageNumber)
            ]
        ]);
    }

    public static InlineKeyboardMarkup UserList(
        IEnumerable<User> users,
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            users.Select(user => new[]
                {
                    Buttons.User(
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        pageNumber)
                })
                .Concat([
                    Shared.Buttons.BuildPaginationInlineButtons(
                        pageNumber,
                        pageCount,
                        number => $"{UserListBo.Button}.{number}",
                        number => $"{UserListBo.Button}.{number}")
                ])
                .Concat([[Buttons.MainPage]]));
    }

    public static InlineKeyboardMarkup UserView(
        long userId,
        int userPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.UserPermissions, Buttons.UserRoles(userId, userPageNumber)],
            [Buttons.BackToUserList(userPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup GameList(
        IEnumerable<Game> games,
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            new[]
                {
                    new[]
                    {
                        Buttons.CreateGame(pageNumber)
                    }
                }
                .Concat(
                    games.Select(game => new[]
                        {
                            Buttons.Game(
                                game.Id,
                                game.Title,
                                pageNumber)
                        })
                        .Concat([
                            Shared.Buttons.BuildPaginationInlineButtons(
                                pageNumber,
                                pageCount,
                                number => $"{GameListBo.Button}.{number}",
                                number => $"{GameListBo.Button}.{number}")
                        ])
                        .Concat([[Buttons.MainPage]])));
    }

    public static InlineKeyboardMarkup GameView(
        int gameId,
        int gamePageNumber,
        int roundsCount,
        int sessionsCount)
    {
        return new InlineKeyboardMarkup([
            [Buttons.EditGame, Buttons.DeleteGame(gameId, gamePageNumber)],
            [Buttons.RoundList(roundsCount, gameId, gamePageNumber)],
            [Buttons.SessionList(sessionsCount, gameId, gamePageNumber)],
            [Buttons.BackToGameList(gamePageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup RoundList(
        IEnumerable<Round> rounds,
        int gameId,
        int gamePageNumber,
        int roundPageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            new[]
                {
                    new[]
                    {
                        Buttons.CreateRound(gameId, gamePageNumber, roundPageNumber)
                    }
                }
                .Concat(
                    rounds.Select(round => new[]
                        {
                            Buttons.Round(
                                round.Id,
                                round.Number,
                                round.Title,
                                gamePageNumber,
                                roundPageNumber)
                        })
                        .Concat([
                            Shared.Buttons.BuildPaginationInlineButtons(
                                roundPageNumber,
                                pageCount,
                                number => $"{RoundListBo.Button}.{gameId}.{gamePageNumber}.{number}",
                                number => $"{RoundListBo.Button}.{gameId}.{gamePageNumber}.{number}")
                        ])
                        .Concat([[Buttons.BackToGame(gameId, gamePageNumber)]])));
    }

    public static InlineKeyboardMarkup RoundView(
        int gameId,
        int roundId,
        IEnumerable<Question> questions,
        int gamePageNumber,
        int roundPageNumber,
        int questionPageNumber,
        int questionPageCount)
    {
        return new InlineKeyboardMarkup(
            new[]
                {
                    new[]
                    {
                        Buttons.CreateQuestion
                    },
                    [Buttons.EditRound, Buttons.DeleteRound]
                }
                .Concat(
                    questions
                        .Select(question => new[]
                        {
                            Buttons.Question(
                                question.Id,
                                question.Number,
                                question.Title,
                                gamePageNumber,
                                roundPageNumber,
                                questionPageNumber)
                        })
                        .Concat([
                            Shared.Buttons.BuildPaginationInlineButtons(
                                questionPageNumber,
                                questionPageCount,
                                number => $"{RoundViewBo.Button}.{roundId}.{gamePageNumber}.{roundPageNumber}.{number}",
                                number => $"{RoundViewBo.Button}.{roundId}.{gamePageNumber}.{roundPageNumber}.{number}")
                        ])
                        .Concat([[Buttons.BackToRoundList(gameId, gamePageNumber, roundPageNumber)]])));
    }

    public static InlineKeyboardMarkup QuestionView(
        Question question,
        int gamePageNumber,
        int roundPageNumber,
        int questionPageNumber)
    {
        return new InlineKeyboardMarkup(
        [
            [Buttons.EditQuestion, Buttons.DeleteQuestion],
            [
                Buttons.QuestionSubmissionNotificationType(
                    question.Id,
                    question.SubmissionNotificationType,
                    gamePageNumber,
                    roundPageNumber,
                    questionPageNumber)
            ],
            [Buttons.BackToRound(question.RoundId, gamePageNumber, roundPageNumber, questionPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup SessionList(
        IEnumerable<Session> sessions,
        int gameId,
        int gamePageNumber,
        int sessionPageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            new[]
                {
                    new[]
                    {
                        Buttons.CreateSession(gameId, gamePageNumber, sessionPageNumber)
                    }
                }
                .Concat(
                    sessions.Select(session => new[]
                        {
                            Buttons.Session(
                                session.Id,
                                session.Name,
                                gamePageNumber,
                                sessionPageNumber)
                        })
                        .Concat([
                            Shared.Buttons.BuildPaginationInlineButtons(
                                sessionPageNumber,
                                pageCount,
                                number => $"{SessionListBo.Button}.{gameId}.{gamePageNumber}.{number}",
                                number => $"{SessionListBo.Button}.{gameId}.{gamePageNumber}.{number}")
                        ])
                        .Concat([[Buttons.BackToGame(gameId, gamePageNumber)]])));
    }

    public static InlineKeyboardMarkup Session(
        int gameId,
        int sessionId,
        int gamePageNumber,
        int sessionPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.SessionGetQr(sessionId, gamePageNumber, sessionPageNumber)],
            [Buttons.EditSession, Buttons.DeleteSession],
            [Buttons.BackToSessionList(gameId, gamePageNumber, sessionPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup DeleteGame(
        int gameId,
        int gamePageNumber)
    {
        int[] fakeIds = [1, 2, 3, 4, 5];
        Random.Shared.Shuffle(fakeIds);
        var confirmationButtons = new[]
            {
                new[]
                {
                    Buttons.DeleteGameConfirmation(gameId, gamePageNumber)
                }
            }
            .Concat(
                fakeIds
                    .Take(3)
                    .Select(fakeId => new[]
                    {
                        Buttons.DeleteGameFakeConfirmation(fakeId, gameId, gamePageNumber)
                    }))
            .ToArray();
        Random.Shared.Shuffle(confirmationButtons);
        return new InlineKeyboardMarkup(
            confirmationButtons
                .Concat([[Buttons.BackToGame(gameId, gamePageNumber)]]));
    }

    public static InlineKeyboardMarkup DeleteMailing(
        int mailingId,
        int mailingPageNumber)
    {
        int[] fakeIds = [1, 2, 3, 4, 5];
        Random.Shared.Shuffle(fakeIds);
        var confirmationButtons = new[]
            {
                new[]
                {
                    Buttons.DeleteMailingConfirmation(mailingId, mailingPageNumber)
                }
            }
            .Concat(
                fakeIds
                    .Take(3)
                    .Select(fakeId => new[]
                    {
                        Buttons.DeleteMailingFakeConfirmation(fakeId, mailingId, mailingPageNumber)
                    }))
            .ToArray();
        Random.Shared.Shuffle(confirmationButtons);
        return new InlineKeyboardMarkup(
            confirmationButtons
                .Concat([[Buttons.BackToMailing(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup UserRoles(
        Role[] roles,
        Role[] userRoles,
        long userId,
        int userPageNumber,
        int userRolesPageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            roles
                .Select(x => (x.Id, x.SystemName, Enabled: userRoles.Any(e => e.Id == x.Id)))
                .Select(role => new[]
                {
                    role.Enabled
                        ? Buttons.UserRoleRevoke(
                            userId,
                            role.Id,
                            role.SystemName,
                            userPageNumber,
                            userRolesPageNumber)
                        : Buttons.UserRoleGrant(
                            userId,
                            role.Id,
                            role.SystemName,
                            userPageNumber,
                            userRolesPageNumber)
                }).Concat([
                    Shared.Buttons.BuildPaginationInlineButtons(
                        userRolesPageNumber,
                        pageCount,
                        number => $"{UserRoleListBo.Button}.{userId}.{userPageNumber}.{number}",
                        number => $"{UserRoleListBo.Button}.{userId}.{userPageNumber}.{number}")
                ])
                .Concat([[Buttons.BackToUser(userId, userPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingList(
        IEnumerable<Mailing> mailings,
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
            new[]
                {
                    new[]
                    {
                        Buttons.CreateMailing(pageNumber)
                    }
                }
                .Concat(
                    mailings.Select(mailing => new[]
                        {
                            Buttons.Mailing(
                                mailing.Id,
                                mailing.Name,
                                pageNumber)
                        })
                        .Concat([
                            Shared.Buttons.BuildPaginationInlineButtons(
                                pageNumber,
                                pageCount,
                                number => $"{MailingListBo.Button}.{number}",
                                number => $"{MailingListBo.Button}.{number}")
                        ])
                        .Concat([[Buttons.MainPage]])));
    }

    public static InlineKeyboardMarkup MailingView(
        int mailingId,
        int mailingPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.EditMailing, Buttons.DeleteMailing(mailingId, mailingPageNumber)],
            [Buttons.MailingPreviewFormatting(mailingId, mailingPageNumber)],
            [Buttons.MailingProfile(mailingId, mailingPageNumber)],
            [Buttons.BackToMailingList(mailingPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup MailingPreview(
        int mailingId,
        int mailingPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.BackToMailing(mailingId, mailingPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup MailingSchema(
        int mailingId,
        int mailingPageNumber,
        int pageNumber,
        int pageCount)
    {
        return new InlineKeyboardMarkup(
        [
            Shared.Buttons.BuildPaginationInlineButtons(
                pageNumber,
                pageCount,
                number => $"{MailingSchemaBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                number => $"{MailingSchemaBo.Button}.{mailingId}.{mailingPageNumber}.{number}"),
            [Buttons.BackToMailingProfile(mailingId, mailingPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup MailingProfile(
        int mailingId,
        int mailingPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.MailingAudience(mailingId, mailingPageNumber)],
            [Buttons.MailingChannel(mailingId, mailingPageNumber)],
            [Buttons.MailingSchema(mailingId, mailingPageNumber, 0)],
            [Buttons.MailingSend(mailingId, mailingPageNumber)],
            [Buttons.BackToMailing(mailingId, mailingPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup MailingAudience(
        int mailingId,
        int mailingPageNumber)
    {
        return new InlineKeyboardMarkup([
            [Buttons.MailingAudienceGames(mailingId, mailingPageNumber, 0)],
            [Buttons.MailingAudienceSessions(mailingId, mailingPageNumber, 0)],
            [Buttons.MailingAudienceTeams(mailingId, mailingPageNumber, 0)],
            [Buttons.MailingAudienceUsers(mailingId, mailingPageNumber, 0)],
            [Buttons.BackToMailingProfile(mailingId, mailingPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup MailingAudienceGames(
        Game[] games,
        Game[] includedGames,
        Game[] excludedGames,
        int gamePageNumber,
        int gamePageCount,
        int mailingId,
        int mailingPageNumber)
    {
        var flags = games.ToDictionary(game => game.Id, game => includedGames.Any(x => x.Id == game.Id)
            ? MailingFilterFlagType.Include
            : excludedGames.Any(x => x.Id == game.Id)
                ? MailingFilterFlagType.Exclude
                : MailingFilterFlagType.None);
        return new InlineKeyboardMarkup(games.Select(game => new[]
            {
                Buttons.MailingAudienceGame(
                    game,
                    flags.GetValueOrDefault(game.Id),
                    mailingId,
                    mailingPageNumber,
                    gamePageNumber)
            })
            .Concat([
                Shared.Buttons.BuildPaginationInlineButtons(
                    gamePageNumber,
                    gamePageCount,
                    number => $"{MailingAudienceGameListBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                    number => $"{MailingAudienceGameListBo.Button}.{mailingId}.{mailingPageNumber}.{number}")
            ])
            .Concat([[Buttons.BackToMailingAudience(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingAudienceSessions(
        Session[] sessions,
        Session[] includedSessions,
        Session[] excludedSessions,
        int sessionPageNumber,
        int sessionPageCount,
        int mailingId,
        int mailingPageNumber)
    {
        var flags = sessions.ToDictionary(session => session.Id, session => includedSessions.Any(x => x.Id == session.Id)
            ? MailingFilterFlagType.Include
            : excludedSessions.Any(x => x.Id == session.Id)
                ? MailingFilterFlagType.Exclude
                : MailingFilterFlagType.None);
        return new InlineKeyboardMarkup(sessions.Select(session => new[]
            {
                Buttons.MailingAudienceSession(
                    session,
                    flags.GetValueOrDefault(session.Id),
                    mailingId,
                    mailingPageNumber,
                    sessionPageNumber)
            })
            .Concat([
                Shared.Buttons.BuildPaginationInlineButtons(
                    sessionPageNumber,
                    sessionPageCount,
                    number => $"{MailingAudienceSessionListBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                    number => $"{MailingAudienceSessionListBo.Button}.{mailingId}.{mailingPageNumber}.{number}")
            ])
            .Concat([[Buttons.BackToMailingAudience(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingAudienceTeams(
        Team[] teams,
        Team[] includedTeams,
        Team[] excludedTeams,
        int teamPageNumber,
        int teamPageCount,
        int mailingId,
        int mailingPageNumber)
    {
        var flags = teams.ToDictionary(team => team.Id, team => includedTeams.Any(x => x.Id == team.Id)
            ? MailingFilterFlagType.Include
            : excludedTeams.Any(x => x.Id == team.Id)
                ? MailingFilterFlagType.Exclude
                : MailingFilterFlagType.None);
        return new InlineKeyboardMarkup(teams.Select(team => new[]
            {
                Buttons.MailingAudienceTeam(
                    team,
                    flags.GetValueOrDefault(team.Id),
                    mailingId,
                    mailingPageNumber,
                    teamPageNumber)
            })
            .Concat([
                Shared.Buttons.BuildPaginationInlineButtons(
                    teamPageNumber,
                    teamPageCount,
                    number => $"{MailingAudienceTeamListBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                    number => $"{MailingAudienceTeamListBo.Button}.{mailingId}.{mailingPageNumber}.{number}")
            ])
            .Concat([[Buttons.BackToMailingAudience(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingAudienceUsers(
        User[] users,
        User[] includedUsers,
        User[] excludedUsers,
        int userPageNumber,
        int userPageCount,
        int mailingId,
        int mailingPageNumber)
    {
        var flags = users.ToDictionary(user => user.Id, user => includedUsers.Any(x => x.Id == user.Id)
            ? MailingFilterFlagType.Include
            : excludedUsers.Any(x => x.Id == user.Id)
                ? MailingFilterFlagType.Exclude
                : MailingFilterFlagType.None);
        return new InlineKeyboardMarkup(users.Select(user => new[]
            {
                Buttons.MailingAudienceUser(
                    user,
                    flags.GetValueOrDefault(user.Id),
                    mailingId,
                    mailingPageNumber,
                    userPageNumber)
            })
            .Concat([
                Shared.Buttons.BuildPaginationInlineButtons(
                    userPageNumber,
                    userPageCount,
                    number => $"{MailingAudienceUserListBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                    number => $"{MailingAudienceUserListBo.Button}.{mailingId}.{mailingPageNumber}.{number}")
            ])
            .Concat([[Buttons.BackToMailingAudience(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingChannel(
        int mailingId,
        int mailingPageNumber,
        MailingProfileContactType contactType)
    {
        return new InlineKeyboardMarkup([
            [Buttons.MailingChannelContactType(mailingId, mailingPageNumber, contactType)],
            [Buttons.MailingChannelBotTypes(mailingId, mailingPageNumber, 0)],
            [Buttons.MailingChannelBots(mailingId, mailingPageNumber, 0)],
            [Buttons.BackToMailingProfile(mailingId, mailingPageNumber)]
        ]);
    }

    public static InlineKeyboardMarkup MailingChannelBotTypes(
        BotType[] botTypes,
        BotType[] includedBotTypes,
        BotType[] excludedBotTypes,
        int botTypePageNumber,
        int botTypePageCount,
        int mailingId,
        int mailingPageNumber)
    {
        var flags = botTypes.ToDictionary(type => type, botType => includedBotTypes.Any(x => x == botType)
            ? MailingFilterFlagType.Include
            : excludedBotTypes.Any(x => x == botType)
                ? MailingFilterFlagType.Exclude
                : MailingFilterFlagType.None);
        return new InlineKeyboardMarkup(botTypes.Select(botType => new[]
            {
                Buttons.MailingChannelBotType(
                    botType,
                    flags.GetValueOrDefault(botType),
                    mailingId,
                    mailingPageNumber,
                    botTypePageNumber)
            })
            .Concat([
                Shared.Buttons.BuildPaginationInlineButtons(
                    botTypePageNumber,
                    botTypePageCount,
                    number => $"{MailingChannelBotTypeListBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                    number => $"{MailingChannelBotTypeListBo.Button}.{mailingId}.{mailingPageNumber}.{number}")
            ])
            .Concat([[Buttons.BackToMailingChannel(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingChannelBots(
        Bot[] bots,
        Bot[] includedBots,
        Bot[] excludedBots,
        int botPageNumber,
        int botPageCount,
        int mailingId,
        int mailingPageNumber)
    {
        var flags = bots.ToDictionary(bot => bot.Id, bot => includedBots.Any(x => x.Id == bot.Id)
            ? MailingFilterFlagType.Include
            : excludedBots.Any(x => x.Id == bot.Id)
                ? MailingFilterFlagType.Exclude
                : MailingFilterFlagType.None);
        return new InlineKeyboardMarkup(bots.Select(bot => new[]
            {
                Buttons.MailingChannelBot(
                    bot,
                    flags.GetValueOrDefault(bot.Id),
                    mailingId,
                    mailingPageNumber,
                    botPageNumber)
            })
            .Concat([
                Shared.Buttons.BuildPaginationInlineButtons(
                    botPageNumber,
                    botPageCount,
                    number => $"{MailingChannelBotListBo.Button}.{mailingId}.{mailingPageNumber}.{number}",
                    number => $"{MailingChannelBotListBo.Button}.{mailingId}.{mailingPageNumber}.{number}")
            ])
            .Concat([[Buttons.BackToMailingChannel(mailingId, mailingPageNumber)]]));
    }

    public static InlineKeyboardMarkup MailingSend(
        int mailingId,
        int mailingPageNumber)
    {
        int[] fakeIds = [1, 2, 3, 4, 5];
        Random.Shared.Shuffle(fakeIds);
        var confirmationButtons = new[]
            {
                new[]
                {
                    Buttons.MailingSendConfirmation(mailingId, mailingPageNumber)
                }
            }
            .Concat(
                fakeIds
                    .Take(3)
                    .Select(fakeId => new[]
                    {
                        Buttons.MailingSendFakeConfirmation(fakeId, mailingId, mailingPageNumber)
                    }))
            .ToArray();
        Random.Shared.Shuffle(confirmationButtons);
        return new InlineKeyboardMarkup(
            confirmationButtons
                .Concat([[Buttons.BackToMailingProfile(mailingId, mailingPageNumber)]]));
    }
}