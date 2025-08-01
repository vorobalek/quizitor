using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using Quizitor.Api.Middleware;
using Quizitor.Api.Services.ExceptionHandler;
using Quizitor.Api.Services.HttpContext.RequestBodyReader;
using Quizitor.Api.Services.HttpContext.RequestCollector;
using Quizitor.Common;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Middleware;

[TestClass]
public class ExceptionHandlerMiddlewareTests
{
    [TestMethod]
    public async Task InvokeAsync_NoExceptionThrown_NoCaptures()
    {
        var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        httpContextMock
            .SetupGet(x => x.Request.Headers)
            .Verifiable(Times.Never);

        var nextMock = new Mock<RequestDelegate>(MockBehavior.Strict);
        nextMock
            .Setup(x => x.Invoke(httpContextMock.Object))
            .Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        var cancellationToken = CancellationToken.None;

        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var httpContextRequestBodyReader = new Mock<IHttpContextRequestBodyReader>(MockBehavior.Strict);
        httpContextRequestBodyReader
            .Setup(x => x.ReadAsync(httpContextMock.Object, cancellationToken))
            .ReturnsAsync(Unique.String())
            .Verifiable(Times.Once);

        var httpContextRequestCollector = new Mock<IHttpContextRequestCollector>(MockBehavior.Strict);
        httpContextRequestCollector
            .Setup(x => x.Collect(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
            .Verifiable(Times.Never);

        var exceptionHandlerServiceMock = new Mock<IExceptionHandlerService>(MockBehavior.Strict);
        exceptionHandlerServiceMock
            .Setup(x => x.Capture(It.IsAny<Exception>(), It.IsAny<object>()))
            .Verifiable(Times.Never);


        await new ExceptionHandlerMiddleware(
                httpContextRequestBodyReader.Object,
                httpContextRequestCollector.Object,
                exceptionHandlerServiceMock.Object,
                globalCancellationTokenSourceMock.Object)
            .InvokeAsync(httpContextMock.Object, nextMock.Object);


        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();

        httpContextRequestBodyReader.VerifyAll();
        httpContextRequestBodyReader.VerifyNoOtherCalls();

        httpContextRequestCollector.VerifyAll();
        httpContextRequestCollector.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        exceptionHandlerServiceMock.VerifyAll();
        exceptionHandlerServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task InvokeAsync_TokenThrowsException_NoCaptures()
    {
        var exception = new Exception();

        var nextMock = new Mock<RequestDelegate>(MockBehavior.Strict);
        nextMock
            .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Verifiable(Times.Never);

        var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        httpContextMock
            .SetupGet(x => x.Request.Headers)
            .Verifiable(Times.Never);

        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Throws(exception)
            .Verifiable(Times.Once);

        var httpContextRequestBodyReader = new Mock<IHttpContextRequestBodyReader>(MockBehavior.Strict);
        httpContextRequestBodyReader
            .Setup(x => x.ReadAsync(It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()))
            .Verifiable(Times.Never);

        var httpContextRequestCollector = new Mock<IHttpContextRequestCollector>(MockBehavior.Strict);
        httpContextRequestCollector
            .Setup(x => x.Collect(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
            .Verifiable(Times.Never);

        var exceptionHandlerServiceMock = new Mock<IExceptionHandlerService>(MockBehavior.Strict);
        exceptionHandlerServiceMock
            .Setup(x => x.Capture(It.IsAny<Exception>(), It.IsAny<object>()))
            .Verifiable(Times.Never);


        var result = await Assert.ThrowsExactlyAsync<Exception>(async () => await
            new ExceptionHandlerMiddleware(
                    httpContextRequestBodyReader.Object,
                    httpContextRequestCollector.Object,
                    exceptionHandlerServiceMock.Object,
                    globalCancellationTokenSourceMock.Object)
                .InvokeAsync(httpContextMock.Object, nextMock.Object));


        Assert.AreEqual(exception, result);

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();

        httpContextRequestBodyReader.VerifyAll();
        httpContextRequestBodyReader.VerifyNoOtherCalls();

        httpContextRequestCollector.VerifyAll();
        httpContextRequestCollector.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        exceptionHandlerServiceMock.VerifyAll();
        exceptionHandlerServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task InvokeAsync_ReaderThrowsException_NoCaptures()
    {
        var exception = new Exception();

        var nextMock = new Mock<RequestDelegate>(MockBehavior.Strict);
        nextMock
            .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Verifiable(Times.Never);

        var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        httpContextMock
            .SetupGet(x => x.Request.Headers)
            .Verifiable(Times.Never);

        var cancellationToken = CancellationToken.None;

        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var httpContextRequestBodyReader = new Mock<IHttpContextRequestBodyReader>(MockBehavior.Strict);
        httpContextRequestBodyReader
            .Setup(x => x.ReadAsync(httpContextMock.Object, cancellationToken))
            .ThrowsAsync(exception)
            .Verifiable(Times.Once);

        var httpContextRequestCollector = new Mock<IHttpContextRequestCollector>(MockBehavior.Strict);
        httpContextRequestCollector
            .Setup(x => x.Collect(It.IsAny<IHeaderDictionary>(), It.IsAny<string>()))
            .Verifiable(Times.Never);

        var exceptionHandlerServiceMock = new Mock<IExceptionHandlerService>(MockBehavior.Strict);
        exceptionHandlerServiceMock
            .Setup(x => x.Capture(It.IsAny<Exception>(), It.IsAny<object>()))
            .Verifiable(Times.Never);


        var result = await Assert.ThrowsExactlyAsync<Exception>(async () => await
            new ExceptionHandlerMiddleware(
                    httpContextRequestBodyReader.Object,
                    httpContextRequestCollector.Object,
                    exceptionHandlerServiceMock.Object,
                    globalCancellationTokenSourceMock.Object)
                .InvokeAsync(httpContextMock.Object, nextMock.Object));


        Assert.AreEqual(exception, result);

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();

        httpContextRequestBodyReader.VerifyAll();
        httpContextRequestBodyReader.VerifyNoOtherCalls();

        httpContextRequestCollector.VerifyAll();
        httpContextRequestCollector.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        exceptionHandlerServiceMock.VerifyAll();
        exceptionHandlerServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task InvokeAsync_NextThrowsException_ExceptionCaptured()
    {
        var exception = new Exception();
        var headers = new HeaderDictionary(new Dictionary<string, StringValues>
        {
            [Unique.String()] = Unique.String()
        });
        var body = Unique.String();
        var context = Unique.String();

        var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        httpContextMock
            .SetupGet(x => x.Request.Headers)
            .Returns(headers)
            .Verifiable(Times.Once);

        var nextMock = new Mock<RequestDelegate>(MockBehavior.Strict);
        nextMock
            .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(exception)
            .Verifiable(Times.Once);

        var cancellationToken = CancellationToken.None;

        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var httpContextRequestBodyReader = new Mock<IHttpContextRequestBodyReader>(MockBehavior.Strict);
        httpContextRequestBodyReader
            .Setup(x => x.ReadAsync(httpContextMock.Object, cancellationToken))
            .ReturnsAsync(body)
            .Verifiable(Times.Once);

        var httpContextRequestCollector = new Mock<IHttpContextRequestCollector>(MockBehavior.Strict);
        httpContextRequestCollector
            .Setup(x => x.Collect(headers, body))
            .Returns(context)
            .Verifiable(Times.Once);

        var exceptionHandlerServiceMock = new Mock<IExceptionHandlerService>(MockBehavior.Strict);
        exceptionHandlerServiceMock
            .Setup(x => x.Capture(exception, context))
            .Verifiable(Times.Once);


        await new ExceptionHandlerMiddleware(
                httpContextRequestBodyReader.Object,
                httpContextRequestCollector.Object,
                exceptionHandlerServiceMock.Object,
                globalCancellationTokenSourceMock.Object)
            .InvokeAsync(httpContextMock.Object, nextMock.Object);


        httpContextMock.VerifyAll();
        httpContextMock.VerifyNoOtherCalls();

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();

        httpContextRequestBodyReader.VerifyAll();
        httpContextRequestBodyReader.VerifyNoOtherCalls();

        httpContextRequestCollector.VerifyAll();
        httpContextRequestCollector.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        exceptionHandlerServiceMock.VerifyAll();
        exceptionHandlerServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task InvokeAsync_NextAndCollectThrowException_NoCaptures()
    {
        var nextException = new Exception();
        var exception = new Exception();
        var headers = new HeaderDictionary(new Dictionary<string, StringValues>
        {
            [Unique.String()] = Unique.String()
        });
        var body = Unique.String();

        var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        httpContextMock
            .SetupGet(x => x.Request.Headers)
            .Returns(headers)
            .Verifiable(Times.Once);

        var nextMock = new Mock<RequestDelegate>(MockBehavior.Strict);
        nextMock
            .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(nextException)
            .Verifiable(Times.Once);

        var cancellationToken = CancellationToken.None;

        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var httpContextRequestBodyReader = new Mock<IHttpContextRequestBodyReader>(MockBehavior.Strict);
        httpContextRequestBodyReader
            .Setup(x => x.ReadAsync(httpContextMock.Object, cancellationToken))
            .ReturnsAsync(body)
            .Verifiable(Times.Once);

        var httpContextRequestCollector = new Mock<IHttpContextRequestCollector>(MockBehavior.Strict);
        httpContextRequestCollector
            .Setup(x => x.Collect(headers, body))
            .Throws(exception)
            .Verifiable(Times.Once);

        var exceptionHandlerServiceMock = new Mock<IExceptionHandlerService>(MockBehavior.Strict);
        exceptionHandlerServiceMock
            .Setup(x => x.Capture(It.IsAny<Exception>(), It.IsAny<object>()))
            .Verifiable(Times.Never);


        var result = await Assert.ThrowsExactlyAsync<Exception>(async () => await
            new ExceptionHandlerMiddleware(
                    httpContextRequestBodyReader.Object,
                    httpContextRequestCollector.Object,
                    exceptionHandlerServiceMock.Object,
                    globalCancellationTokenSourceMock.Object)
                .InvokeAsync(httpContextMock.Object, nextMock.Object));


        Assert.AreEqual(exception, result);

        httpContextMock.VerifyAll();
        httpContextMock.VerifyNoOtherCalls();

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();

        httpContextRequestBodyReader.VerifyAll();
        httpContextRequestBodyReader.VerifyNoOtherCalls();

        httpContextRequestCollector.VerifyAll();
        httpContextRequestCollector.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        exceptionHandlerServiceMock.VerifyAll();
        exceptionHandlerServiceMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public async Task InvokeAsync_NextAndCaptureThrowException_NoCaptures()
    {
        var nextException = new Exception();
        var exception = new Exception();
        var headers = new HeaderDictionary(new Dictionary<string, StringValues>
        {
            [Unique.String()] = Unique.String()
        });
        var body = Unique.String();
        var context = Unique.String();

        var httpContextMock = new Mock<HttpContext>(MockBehavior.Strict);
        httpContextMock
            .SetupGet(x => x.Request.Headers)
            .Returns(headers)
            .Verifiable(Times.Once);

        var nextMock = new Mock<RequestDelegate>(MockBehavior.Strict);
        nextMock
            .Setup(x => x.Invoke(It.IsAny<HttpContext>()))
            .Throws(nextException)
            .Verifiable(Times.Once);

        var cancellationToken = CancellationToken.None;

        var globalCancellationTokenSourceMock = new Mock<IGlobalCancellationTokenSource>(MockBehavior.Strict);
        globalCancellationTokenSourceMock
            .SetupGet(x => x.Token)
            .Returns(cancellationToken)
            .Verifiable(Times.Once);

        var httpContextRequestBodyReader = new Mock<IHttpContextRequestBodyReader>(MockBehavior.Strict);
        httpContextRequestBodyReader
            .Setup(x => x.ReadAsync(httpContextMock.Object, cancellationToken))
            .ReturnsAsync(body)
            .Verifiable(Times.Once);

        var httpContextRequestCollector = new Mock<IHttpContextRequestCollector>(MockBehavior.Strict);
        httpContextRequestCollector
            .Setup(x => x.Collect(headers, body))
            .Returns(context)
            .Verifiable(Times.Once);

        var exceptionHandlerServiceMock = new Mock<IExceptionHandlerService>(MockBehavior.Strict);
        exceptionHandlerServiceMock
            .Setup(x => x.Capture(nextException, context))
            .Throws(exception)
            .Verifiable(Times.Once);


        var result = await Assert.ThrowsExactlyAsync<Exception>(async () => await
            new ExceptionHandlerMiddleware(
                    httpContextRequestBodyReader.Object,
                    httpContextRequestCollector.Object,
                    exceptionHandlerServiceMock.Object,
                    globalCancellationTokenSourceMock.Object)
                .InvokeAsync(httpContextMock.Object, nextMock.Object));


        Assert.AreEqual(exception, result);

        httpContextMock.VerifyAll();
        httpContextMock.VerifyNoOtherCalls();

        nextMock.VerifyAll();
        nextMock.VerifyNoOtherCalls();

        httpContextRequestBodyReader.VerifyAll();
        httpContextRequestBodyReader.VerifyNoOtherCalls();

        httpContextRequestCollector.VerifyAll();
        httpContextRequestCollector.VerifyNoOtherCalls();

        globalCancellationTokenSourceMock.VerifyAll();
        globalCancellationTokenSourceMock.VerifyNoOtherCalls();

        exceptionHandlerServiceMock.VerifyAll();
        exceptionHandlerServiceMock.VerifyNoOtherCalls();
    }
}