namespace Quizitor.Common;

public interface IGlobalCancellationTokenSource
{
    CancellationToken Token { get; }
}