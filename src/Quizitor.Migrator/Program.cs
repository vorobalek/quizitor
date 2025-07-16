using Quizitor.Common;
using Quizitor.Data;
using Quizitor.Localization;
using Quizitor.Migrator;
using Quizitor.Migrator.Configuration;
using Quizitor.Migrator.Extensions;
using Quizitor.Migrator.Seeds;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder
        .AddGlobalCancellationToken()
        .AddLocalization(AppConfiguration.Locale)
        .AddDatabase(AppConfiguration.DbConnectionString)
        .ConfigureServices(services => services
            .AddMigrator()
            .AddSeed<RoleSeed>()
            .AddSeed<BotCommandSeed>()
            .AddSeed<TestDataSeed>()))
    .Build();

await using var asyncScope = host
    .Services
    .CreateAsyncScope();

await asyncScope
    .ServiceProvider
    .GetRequiredService<IMigrator>()
    .MigrateAsync();