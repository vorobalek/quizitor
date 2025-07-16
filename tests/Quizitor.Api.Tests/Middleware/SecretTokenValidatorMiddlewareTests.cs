using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Quizitor.Api.Middleware;
using Quizitor.Api.Options;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Middleware;

[TestClass]
public sealed class SecretTokenValidatorMiddlewareTests
{
    [TestMethod]
    public async Task InvokeAsync_ValidToken_InvokesNextMiddleware()
    {
        var headerName = Unique.String();
        var token = Unique.String();

        var httpRequest = new Mock<HttpRequest>(MockBehavior.Strict);
        httpRequest
            .SetupGet(x => x.Headers)
            .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
            {
                {
                    headerName, token
                }
            }))
            .Verifiable(Times.Once);

        var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
        httpContext
            .SetupGet(x => x.Request)
            .Returns(httpRequest.Object)
            .Verifiable(Times.Once);

        var nextMock = new Mock<RequestDelegate>();
        nextMock
            .Setup(x => x.Invoke(httpContext.Object))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);


        await new SecretTokenValidatorMiddleware(
                new OptionsWrapper<TelegramBotSecrets>(
                    new TelegramBotSecrets
                    {
                        HeaderName = headerName,
                        Token = token
                    }))
            .InvokeAsync(
                httpContext.Object,
                nextMock.Object);


        httpRequest.VerifyAll();
        httpRequest.VerifyNoOtherCalls();

        httpContext.VerifyAll();
        httpContext.VerifyNoOtherCalls();

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task InvokeAsync_InvalidToken_Responses401()
    {
        var headerName = Unique.String();
        var httpRequest = new Mock<HttpRequest>(MockBehavior.Strict);
        httpRequest
            .SetupGet(x => x.Headers)
            .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
            {
                {
                    headerName, Unique.String()
                }
            }))
            .Verifiable(Times.Once);

        var httpResponse = new Mock<HttpResponse>(MockBehavior.Strict);
        httpResponse
            .SetupSet(x => x.StatusCode = StatusCodes.Status401Unauthorized)
            .Verifiable(Times.Once);
        httpResponse
            .Setup(x => x.CompleteAsync())
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        var httpContext = new Mock<HttpContext>(MockBehavior.Strict);
        httpContext
            .SetupGet(x => x.Request)
            .Returns(httpRequest.Object)
            .Verifiable(Times.Once);
        httpContext
            .SetupGet(x => x.Response)
            .Returns(httpResponse.Object)
            .Verifiable(Times.Exactly(2));

        var nextMock = new Mock<RequestDelegate>();
        nextMock
            .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Verifiable(Times.Never);


        await new SecretTokenValidatorMiddleware(
                new OptionsWrapper<TelegramBotSecrets>(
                    new TelegramBotSecrets
                    {
                        HeaderName = headerName,
                        Token = Unique.String()
                    }))
            .InvokeAsync(
                httpContext.Object,
                nextMock.Object);


        httpRequest.VerifyAll();
        httpRequest.VerifyNoOtherCalls();

        httpResponse.VerifyAll();
        httpResponse.VerifyNoOtherCalls();

        httpContext.VerifyAll();
        httpContext.VerifyNoOtherCalls();

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();
    }
}