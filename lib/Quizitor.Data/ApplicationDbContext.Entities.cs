using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Entities.Events;

namespace Quizitor.Data;

public partial class ApplicationDbContext
{
    public DbSet<Bot> Bots { get; set; }
    public DbSet<BotCommand> BotCommands { get; set; }
    public DbSet<BotInteraction> BotInteractions { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<UserPrompt> UserPrompts { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<TeamLeader> TeamLeaders { get; set; }

    public DbSet<Game> Games { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<QuestionRule> QuestionRules { get; set; }
    public DbSet<QuestionTiming> QuestionTimings { get; set; }
    public DbSet<QuestionTimingStop> QuestionStopTimings { get; set; }
    public DbSet<QuestionTimingNotify> QuestionNotifyTimings { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<Mailing> Mailings { get; set; }
    public DbSet<MailingProfile> MailingProfiles { get; set; }
    public DbSet<MailingFilterGame> MailingFilterGames { get; set; }
    public DbSet<MailingFilterSession> MailingFilterSessions { get; set; }
    public DbSet<MailingFilterTeam> MailingFilterTeams { get; set; }
    public DbSet<MailingFilterUser> MailingFilterUsers { get; set; }
    public DbSet<MailingFilterBot> MailingFilterBots { get; set; }
}