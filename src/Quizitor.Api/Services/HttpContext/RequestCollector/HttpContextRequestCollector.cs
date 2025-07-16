namespace Quizitor.Api.Services.HttpContext.RequestCollector;

internal sealed class HttpContextRequestCollector : IHttpContextRequestCollector
{
    public object Collect(IHeaderDictionary headers, string body)
    {
        return new
        {
            Headers = headers.ToDictionary(
                x => x.Key,
                x => x.Value.ToString()),
            Body = body
        };
    }
}