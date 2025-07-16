using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sentry.Protocol.Envelopes;

namespace Quizitor.Logging;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder AddLogging(
        this IWebHostBuilder builder,
        LogLevel logLevel = LogLevel.Error,
        string? dsn = null)
    {
        builder.ConfigureLogging(logging => logging
            .AddConsole());
        return
            !string.IsNullOrWhiteSpace(dsn)
                ? builder.ConfigureLogging(logging =>
                {
                    logging.AddSentry(configuration =>
                    {
                        configuration.Dsn = dsn;
                        configuration.MinimumEventLevel = logLevel;
                        configuration.DisableDuplicateEventDetection();
                    });
                })
                : builder
                    .ConfigureServices(services => services
                        .AddSingleton<IHub, HubStub>());
    }

    internal sealed class HubStub(ILogger<HubStub> logger) : IHub
    {
        public bool CaptureEnvelope(Envelope envelope)
        {
            throw new NotImplementedException();
        }

        public SentryId CaptureEvent(SentryEvent evt, Scope? scope = null, SentryHint? hint = null)
        {
            throw new NotImplementedException();
        }

        public void CaptureFeedback(SentryFeedback feedback, Scope? scope = null, SentryHint? hint = null)
        {
            throw new NotImplementedException();
        }

        [Obsolete("Obsolete")]
        public void CaptureUserFeedback(UserFeedback userFeedback)
        {
            throw new NotImplementedException();
        }

        public void CaptureTransaction(SentryTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public void CaptureTransaction(SentryTransaction transaction, Scope? scope, SentryHint? hint)
        {
            throw new NotImplementedException();
        }

        public void CaptureSession(SessionUpdate sessionUpdate)
        {
            throw new NotImplementedException();
        }

        public SentryId CaptureCheckIn(string monitorSlug, CheckInStatus status, SentryId? sentryId = null, TimeSpan? duration = null, Scope? scope = null, Action<SentryMonitorOptions>? configureMonitorOptions = null)
        {
            throw new NotImplementedException();
        }

        public Task FlushAsync(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled { get; }
        public void ConfigureScope(Action<Scope> configureScope)
        {
            throw new NotImplementedException();
        }

        public void ConfigureScope<TArg>(Action<Scope, TArg> configureScope, TArg arg)
        {
            throw new NotImplementedException();
        }

        public Task ConfigureScopeAsync(Func<Scope, Task> configureScope)
        {
            throw new NotImplementedException();
        }

        public Task ConfigureScopeAsync<TArg>(Func<Scope, TArg, Task> configureScope, TArg arg)
        {
            throw new NotImplementedException();
        }

        public void SetTag(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void UnsetTag(string key)
        {
            throw new NotImplementedException();
        }

        public void BindClient(ISentryClient client)
        {
            throw new NotImplementedException();
        }

        public IDisposable PushScope()
        {
            throw new NotImplementedException();
        }

        public IDisposable PushScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public ITransactionTracer StartTransaction(ITransactionContext context, IReadOnlyDictionary<string, object?> customSamplingContext)
        {
            throw new NotImplementedException();
        }

        public void BindException(Exception exception, ISpan span)
        {
            throw new NotImplementedException();
        }

        public ISpan? GetSpan()
        {
            throw new NotImplementedException();
        }

        public SentryTraceHeader? GetTraceHeader()
        {
            throw new NotImplementedException();
        }

        public BaggageHeader? GetBaggage()
        {
            throw new NotImplementedException();
        }

        public TransactionContext ContinueTrace(string? traceHeader, string? baggageHeader, string? name = null, string? operation = null)
        {
            throw new NotImplementedException();
        }

        public TransactionContext ContinueTrace(SentryTraceHeader? traceHeader, BaggageHeader? baggageHeader, string? name = null, string? operation = null)
        {
            throw new NotImplementedException();
        }

        public void StartSession()
        {
            throw new NotImplementedException();
        }

        public void PauseSession()
        {
            throw new NotImplementedException();
        }

        public void ResumeSession()
        {
            throw new NotImplementedException();
        }

        public void EndSession(SessionEndStatus status = SessionEndStatus.Exited)
        {
            throw new NotImplementedException();
        }

        public SentryId CaptureEvent(SentryEvent evt, Action<Scope> configureScope)
        {
            logger.LogError(evt.Exception, "Unhandled exception");
            return SentryId.Empty;
        }

        public SentryId CaptureEvent(SentryEvent evt, SentryHint? hint, Action<Scope> configureScope)
        {
            throw new NotImplementedException();
        }

        public void CaptureFeedback(SentryFeedback feedback, Action<Scope> configureScope, SentryHint? hint = null)
        {
            throw new NotImplementedException();
        }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public SentryId LastEventId { get; }
    }
}