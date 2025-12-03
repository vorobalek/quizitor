using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Entities.Events;

namespace Quizitor.Data;

public partial class ApplicationDbContext
{
    public required DbSet<Bot> Bots { get; set; }
    public required DbSet<BotCommand> BotCommands { get; set; }
    public required DbSet<BotInteraction> BotInteractions { get; set; }

    public required DbSet<Role> Roles { get; set; }
    public required DbSet<RolePermission> RolePermissions { get; set; }

    public required DbSet<User> Users { get; set; }
    public required DbSet<UserPrompt> UserPrompts { get; set; }
    public required DbSet<UserPermission> UserPermissions { get; set; }

    public required DbSet<Team> Teams { get; set; }
    public required DbSet<TeamMember> TeamMembers { get; set; }
    public required DbSet<TeamLeader> TeamLeaders { get; set; }

    public required DbSet<Game> Games { get; set; }
    public required DbSet<Round> Rounds { get; set; }
    public required DbSet<Question> Questions { get; set; }
    public required DbSet<QuestionOption> QuestionOptions { get; set; }
    public required DbSet<QuestionRule> QuestionRules { get; set; }
    public required DbSet<QuestionTiming> QuestionTimings { get; set; }
    public required DbSet<QuestionTimingStop> QuestionStopTimings { get; set; }
    public required DbSet<QuestionTimingNotify> QuestionNotifyTimings { get; set; }
    public required DbSet<Session> Sessions { get; set; }
    public required DbSet<Submission> Submissions { get; set; }
    public required DbSet<Mailing> Mailings { get; set; }
    public required DbSet<MailingProfile> MailingProfiles { get; set; }
    public required DbSet<MailingFilterGame> MailingFilterGames { get; set; }
    public required DbSet<MailingFilterSession> MailingFilterSessions { get; set; }
    public required DbSet<MailingFilterTeam> MailingFilterTeams { get; set; }
    public required DbSet<MailingFilterUser> MailingFilterUsers { get; set; }
    public required DbSet<MailingFilterBot> MailingFilterBots { get; set; }
}