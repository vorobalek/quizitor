using Microsoft.AspNetCore.Http;
using Moq;
using Quizitor.Api.Services.ExceptionHandler;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Services.ExceptionHandler;

[TestClass]
public class ExceptionHandlerServiceTests
{
    [TestMethod]
    public void Capture_HappyPath()
    {
        var exception = new Exception();
        var context = Unique.String();
        var scope = new Scope(null);
        SentryEvent? capturedEvent = null;

        var hubMock = new Mock<IHub>(MockBehavior.Strict);
        hubMock
            .Setup(x => x.CaptureEvent(It.IsAny<SentryEvent>(), It.IsAny<Action<Scope>>()))
            .Returns<SentryEvent, Action<Scope>>((sentryEvent, action) =>
            {
                capturedEvent = sentryEvent;
                action?.Invoke(scope);
                return new SentryId(Unique.Guid());
            })
            .Verifiable(Times.Once);


        new ExceptionHandlerService(hubMock.Object).Capture(exception, context);


        Assert.IsNotNull(capturedEvent?.Exception);
        Assert.AreEqual(exception, capturedEvent.Exception);
        Assert.IsTrue(scope.Contexts.ContainsKey(nameof(HttpRequest)));
        Assert.AreEqual(context, scope.Contexts[nameof(HttpRequest)]);
        hubMock.VerifyAll();
        hubMock.VerifyNoOtherCalls();
    }
}