namespace Quizitor.Api.Services.HttpContext.RequestCollector;

public interface IHttpContextRequestCollector
{
    object Collect(IHeaderDictionary headers, string body);
}