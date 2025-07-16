namespace Quizitor.Migrator;

internal interface IMigrator
{
    Task MigrateAsync();
}