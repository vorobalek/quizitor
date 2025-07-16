namespace Quizitor.Api.Services.ExceptionHandler;

internal sealed class ExceptionHandlerService(IHub sentry) : IExceptionHandlerService
{
    public void Capture(Exception exception, object context)
    {
        sentry.CaptureException(exception, scope =>
        {
            scope.Contexts[nameof(HttpRequest)] = context;
        });
    }
}