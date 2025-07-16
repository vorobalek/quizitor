namespace Quizitor.Migrator.Seeds;

internal interface ISeed
{
    Task ApplyAsync(CancellationToken cancellationToken);
}