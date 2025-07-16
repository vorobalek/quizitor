namespace Quizitor.Api.Services.HttpContext.RequestBodyReader;

public interface IHttpContextRequestBodyReader
{
    Task<string> ReadAsync(Microsoft.AspNetCore.Http.HttpContext httpContext, CancellationToken cancellationToken);
}