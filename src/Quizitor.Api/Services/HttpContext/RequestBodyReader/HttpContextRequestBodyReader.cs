namespace Quizitor.Api.Services.HttpContext.RequestBodyReader;

internal sealed class HttpContextRequestBodyReader : IHttpContextRequestBodyReader
{
    public async Task<string> ReadAsync(Microsoft.AspNetCore.Http.HttpContext httpContext, CancellationToken cancellationToken)
    {
        httpContext.Request.EnableBuffering();
        using var streamReader = new StreamReader(
            httpContext.Request.Body,
            leaveOpen: true);
        var body = await streamReader.ReadToEndAsync(cancellationToken);
        httpContext.Request.Body.Position = 0;

        return body;
    }
}