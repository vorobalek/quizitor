namespace Quizitor.Kafka;

public class RetryLaterExtension(
    int delay,
    string reason) : Exception(reason)
{
    public int Delay { get; } = delay;
}