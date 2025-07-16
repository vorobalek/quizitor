namespace Quizitor.Bots.Exceptions;

public class UnauthorizedException(string[] missingPermissions)
    : Exception($"Unauthorized. Missing permissions: {string.Join(", ", missingPermissions)}.")
{
    public string[] MissingPermissions { get; private set; } = missingPermissions;
}