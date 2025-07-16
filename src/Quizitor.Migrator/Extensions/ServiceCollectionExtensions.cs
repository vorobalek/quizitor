using Quizitor.Migrator.Seeds;

namespace Quizitor.Migrator.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrator(this IServiceCollection services)
    {
        return services.AddScoped<IMigrator, Migrator>();
    }

    public static IServiceCollection AddSeed<TSeed>(this IServiceCollection services)
        where TSeed : class, ISeed
    {
        return services.AddScoped<ISeed, TSeed>();
    }
}