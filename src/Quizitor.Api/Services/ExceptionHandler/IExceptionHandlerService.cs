namespace Quizitor.Api.Services.ExceptionHandler;

public interface IExceptionHandlerService
{
    void Capture(Exception exception, object context);
}