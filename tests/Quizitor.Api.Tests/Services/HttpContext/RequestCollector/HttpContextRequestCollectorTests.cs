using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Quizitor.Api.Services.HttpContext.RequestCollector;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Services.HttpContext.RequestCollector;

[TestClass]
public class HttpContextRequestCollectorTests
{
    [TestMethod]
    public void Collect_HappyPath()
    {
        var headers = new HeaderDictionary(new Dictionary<string, StringValues>
        {
            [Unique.String()] = new([Unique.String(), Unique.String()]),
            [Unique.String()] = new([Unique.String(), Unique.String()]),
            [Unique.String()] = new([Unique.String(), Unique.String()])
        });
        var body = Unique.String();


        dynamic result = new HttpContextRequestCollector()
            .Collect(headers, body);


        Assert.IsNotNull(result);
        Assert.AreEqual(result.Body, body);
        Assert.IsInstanceOfType<IDictionary<string, string>>(result.Headers);
        var headersDictionary = (result.Headers as IDictionary<string, string>)!;
        Assert.IsTrue(headers.Keys.SequenceEqual(headersDictionary.Keys));
        foreach (var header in headers) Assert.AreEqual(headers[header.Key].ToString(), headersDictionary[header.Key]);
    }
}