using Quizitor.Api.Services.ExceptionHandler;
using Quizitor.Api.Services.HttpContext.RequestBodyReader;
using Quizitor.Api.Services.HttpContext.RequestCollector;
using Quizitor.Common;

namespace Quizitor.Api.Middleware;

internal sealed class ExceptionHandlerMiddleware(
    IHttpContextRequestBodyReader httpContextRequestBodyReader,
    IHttpContextRequestCollector httpContextRequestCollector,
    IExceptionHandlerService exceptionHandlerService,
    IGlobalCancellationTokenSource globalCancellationTokenSource) :
    IMiddleware
{
    public async Task InvokeAsync(
        HttpContext httpContext,
        RequestDelegate next)
    {
        var body = await httpContextRequestBodyReader.ReadAsync(httpContext, globalCancellationTokenSource.Token);
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            var context = httpContextRequestCollector.Collect(
                httpContext.Request.Headers,
                body);
            exceptionHandlerService.Capture(ex, context);
        }
    }
}