using Quizitor.Migrator.Seeds;

namespace Quizitor.Migrator.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMigrator()
        {
            return services.AddScoped<IMigrator, Migrator>();
        }

        public IServiceCollection AddSeed<TSeed>()
            where TSeed : class, ISeed
        {
            return services.AddScoped<ISeed, TSeed>();
        }
    }
}