namespace Quizitor.Migrator.Seeds;

// ReSharper disable once ClassNeverInstantiated.Global
public class TestDataSeed : ISeed
{
    public Task ApplyAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}