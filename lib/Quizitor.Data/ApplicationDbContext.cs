using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Quizitor.Data.Entities;
using Quizitor.Data.Enums;

namespace Quizitor.Data;

public sealed partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(builder =>
            {
                builder.MigrationsHistoryTable("__ef_migrations_history");
                builder.MigrationsAssembly("Quizitor.Migrator");
            })
            .UseSnakeCaseNamingConvention();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<UserPrompt>()
            .HasIndex(x => new
            {
                x.UserId,
                x.BotId
            })
            .AreNullsDistinct();

        modelBuilder
            .Entity<User>()
            .HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<UserRole>()
            .ToTable("user_role");

        modelBuilder
            .Entity<Question>()
            .Property(x => x.Attempts)
            .HasDefaultValue(1);

        modelBuilder
            .Entity<QuestionRule>()
            .Property<string>("type")
            .HasMaxLength(32);

        modelBuilder
            .Entity<QuestionRule>()
            .HasDiscriminator<string>("type")
            .HasValue<AnyAnswerQuestionRule>("any_answer")
            .HasValue<FirstAcceptedAnswerQuestionRule>("first_accepted_answer");

        modelBuilder
            .Entity<MailingProfile>()
            .Property(x => x.BotTypes)
            .HasConversion(
                x => JsonSerializer.Serialize(x, JsonSerializerOptions.Default),
                x => JsonSerializer.Deserialize<Dictionary<BotType, MailingFilterFlagType>>(x, JsonSerializerOptions.Default)!);

        modelBuilder.Entity<AnyAnswerQuestionRule>();
    }
}