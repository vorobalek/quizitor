using System.Text;
using Microsoft.AspNetCore.Http;
using Quizitor.Api.Services.HttpContext.RequestBodyReader;
using Quizitor.Tests;

namespace Quizitor.Api.Tests.Services.HttpContext.RequestBodyReader;

[TestClass]
public class HttpContextRequestBodyReaderTests
{
    [TestMethod]
    public async Task ReadAsync_HappyPath()
    {
        var body = Unique.String();
        var innerStream = new MemoryStream(Encoding.UTF8.GetBytes(body));
        var nonSeekableStream = new NonSeekableStream(innerStream);
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Body = nonSeekableStream
            }
        };

        var cancellationToken = CancellationToken.None;


        var result = await new HttpContextRequestBodyReader()
            .ReadAsync(httpContext, cancellationToken);


        Assert.AreEqual(body, result);
        Assert.IsTrue(httpContext.Request.Body.CanRead);
        Assert.AreEqual(0, httpContext.Request.Body.Position);
    }

    [TestMethod]
    public void NonSeekableStream_Tests()
    {
        var body = Unique.String();
        var innerStream = new MemoryStream();
        var nonSeekableStream = new NonSeekableStream(innerStream);
        nonSeekableStream.Write(Encoding.UTF8.GetBytes(body));
        nonSeekableStream.Flush();


        Assert.AreEqual(innerStream.CanRead, nonSeekableStream.CanRead);
        Assert.IsFalse(nonSeekableStream.CanSeek);
        Assert.AreEqual(innerStream.CanWrite, nonSeekableStream.CanWrite);
        Assert.ThrowsException<NotSupportedException>(() => nonSeekableStream.Length);
        Assert.ThrowsException<NotSupportedException>(() => nonSeekableStream.Position);
        Assert.ThrowsException<NotSupportedException>(() => nonSeekableStream.Position = 0);
        Assert.ThrowsException<NotSupportedException>(() => nonSeekableStream.Seek(0, SeekOrigin.Begin));
        Assert.ThrowsException<NotSupportedException>(() => nonSeekableStream.SetLength(0));
    }

    private class NonSeekableStream(Stream inner) : Stream
    {
        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => inner.CanWrite;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            inner.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return inner.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            inner.Write(buffer, offset, count);
        }
    }
}