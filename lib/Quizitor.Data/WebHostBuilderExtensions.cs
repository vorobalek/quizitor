using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quizitor.Data.Repositories;
using Quizitor.Data.Repositories.Internals;

namespace Quizitor.Data;

public static class WebHostBuilderExtensions
{
    extension(IWebHostBuilder builder)
    {
        public IWebHostBuilder AddDatabase(string? connectionString = null)
        {
            return builder.ConfigureServices((context, services) =>
            {
                services
                    .AddScoped<IDbContextProvider, DbContextProvider>()
                    .AddScoped<IBotRepository, BotRepository>()
                    .AddScoped<IBotCommandRepository, BotCommandRepository>()
                    .AddScoped<IBotInteractionRepository, BotInteractionRepository>()
                    .AddScoped<IGameRepository, GameRepository>()
                    .AddScoped<IRoundRepository, RoundRepository>()
                    .AddScoped<IQuestionRepository, QuestionRepository>()
                    .AddScoped<IQuestionTimingRepository, QuestionTimingRepository>()
                    .AddScoped<IQuestionNotifyTimingRepository, QuestionNotifyTimingRepository>()
                    .AddScoped<IQuestionStopTimingRepository, QuestionStopTimingRepository>()
                    .AddScoped<ISessionRepository, SessionRepository>()
                    .AddScoped<ISubmissionRepository, SubmissionRepository>()
                    .AddScoped<IRoleRepository, RoleRepository>()
                    .AddScoped<IUserRepository, UserRepository>()
                    .AddScoped<ITeamRepository, TeamRepository>()
                    .AddScoped<IMailingRepository, MailingRepository>()
                    .AddDbContext<ApplicationDbContext>(optionsBuilder => optionsBuilder
                        .UseNpgsql(
                            connectionString ?? context.Configuration.GetConnectionString("Default"),
                            options => options
                                .MigrationsAssembly("Quizitor.Migrator")
                                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            });
        }
    }
}