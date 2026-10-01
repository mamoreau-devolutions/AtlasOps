namespace AtlasOps.Connectors.Runtime;

using System.Collections.Concurrent;
using System.Diagnostics;

using AtlasOps.Connectors.Contracts;

public sealed class ConnectorRuntime
{
    private static readonly IReadOnlySet<string> SensitiveParameterNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "authorization",
            "password",
            "secret",
            "token",
            "apiKey",
            "connectionString",
        };

    private readonly ConnectorRegistry registry;
    private readonly ConnectorRateLimiter rateLimiter;
    private readonly IConnectorAuditSink auditSink;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> concurrencyGates =
        new(StringComparer.OrdinalIgnoreCase);

    public ConnectorRuntime(
        ConnectorRegistry registry,
        ConnectorRateLimiter rateLimiter,
        IConnectorAuditSink auditSink)
    {
        this.registry = registry;
        this.rateLimiter = rateLimiter;
        this.auditSink = auditSink;
    }

    public async ValueTask<ConnectorExecutionOutcome> ExecuteAsync(
        ConnectorExecutionRequest request,
        CancellationToken cancellationToken)
    {
        DateTimeOffset startedAt = DateTimeOffset.UtcNow;

        if (!this.registry.TryResolve(request.ConnectorId, out ConnectorDefinition? definition, out IConnectorHandler? handler) ||
            definition is null ||
            handler is null)
        {
            ConnectorExecutionOutcome notFound = CreateOutcome(
                request,
                ConnectorExecutionStatus.NotFound,
                "connector-not-found",
                "The requested connector is not registered.",
                0,
                startedAt,
                ConnectorContract.EmptyDetails);
            await this.PublishAuditAsync(request, notFound, CancellationToken.None).ConfigureAwait(false);
            return notFound;
        }

        if (!definition.Enabled)
        {
            ConnectorExecutionOutcome disabled = CreateOutcome(
                request,
                ConnectorExecutionStatus.Disabled,
                "connector-disabled",
                "The requested connector is disabled.",
                0,
                startedAt,
                ConnectorContract.EmptyDetails);
            await this.PublishAuditAsync(request, disabled, CancellationToken.None).ConfigureAwait(false);
            return disabled;
        }

        if (!this.rateLimiter.TryAcquire(definition.Id, definition.RequestsPerMinute, out TimeSpan retryAfter))
        {
            Dictionary<string, string> details = new(StringComparer.OrdinalIgnoreCase)
            {
                ["retryAfterMilliseconds"] = Math.Ceiling(retryAfter.TotalMilliseconds).ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
            };
            ConnectorExecutionOutcome throttled = CreateOutcome(
                request,
                ConnectorExecutionStatus.Throttled,
                "rate-limit-exceeded",
                "The connector request rate limit was exceeded.",
                0,
                startedAt,
                details);
            await this.PublishAuditAsync(request, throttled, CancellationToken.None).ConfigureAwait(false);
            return throttled;
        }

        SemaphoreSlim concurrencyGate = this.concurrencyGates.GetOrAdd(
            definition.Id,
            static (_, maximumConcurrency) => new SemaphoreSlim(maximumConcurrency, maximumConcurrency),
            definition.MaximumConcurrency);

        await concurrencyGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        ConnectorExecutionOutcome outcome;
        try
        {
            outcome = await ExecuteWithRetriesAsync(
                definition,
                handler,
                request,
                startedAt,
                cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            concurrencyGate.Release();
        }

        await this.PublishAuditAsync(request, outcome, CancellationToken.None).ConfigureAwait(false);
        return outcome;
    }

    private static async ValueTask<ConnectorExecutionOutcome> ExecuteWithRetriesAsync(
        ConnectorDefinition definition,
        IConnectorHandler handler,
        ConnectorExecutionRequest request,
        DateTimeOffset startedAt,
        CancellationToken cancellationToken)
    {
        Exception? lastException = null;

        for (int attempt = 1; attempt <= definition.MaximumAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using CancellationTokenSource timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutSource.CancelAfter(definition.Timeout);

            try
            {
                ConnectorExecutionOutcome handlerOutcome = await handler.ExecuteAsync(
                    request,
                    timeoutSource.Token).ConfigureAwait(false);

                ConnectorExecutionOutcome normalized = handlerOutcome with
                {
                    ExecutionId = request.ExecutionId,
                    Attempts = attempt,
                    StartedAt = startedAt,
                    CompletedAt = DateTimeOffset.UtcNow,
                    Details = Redact(handlerOutcome.Details),
                };

                if (normalized.Status == ConnectorExecutionStatus.Succeeded ||
                    attempt == definition.MaximumAttempts)
                {
                    return normalized;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (attempt < definition.MaximumAttempts)
            {
                lastException = exception;
                TimeSpan delay = TimeSpan.FromMilliseconds(Math.Min(2_000, 100 * Math.Pow(2, attempt - 1)));
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                lastException = exception;
            }
        }

        Dictionary<string, string> details = new(StringComparer.OrdinalIgnoreCase);
        if (lastException is not null)
        {
            details["exceptionType"] = lastException.GetType().FullName ?? lastException.GetType().Name;
        }

        return CreateOutcome(
            request,
            ConnectorExecutionStatus.Failed,
            "execution-failed",
            lastException?.Message ?? "The connector failed after all configured attempts.",
            definition.MaximumAttempts,
            startedAt,
            details);
    }

    private async ValueTask PublishAuditAsync(
        ConnectorExecutionRequest request,
        ConnectorExecutionOutcome outcome,
        CancellationToken cancellationToken)
    {
        ConnectorAuditRecord record = new(
            Guid.NewGuid(),
            outcome.ExecutionId,
            request.ConnectorId,
            request.Operation,
            request.ResourceId,
            outcome.Status,
            outcome.Attempts,
            outcome.CompletedAt,
            outcome.Duration,
            request.CorrelationId ?? Activity.Current?.TraceId.ToString(),
            Redact(outcome.Details));
        await this.auditSink.PublishAsync(record, cancellationToken).ConfigureAwait(false);
    }

    private static ConnectorExecutionOutcome CreateOutcome(
        ConnectorExecutionRequest request,
        ConnectorExecutionStatus status,
        string code,
        string message,
        int attempts,
        DateTimeOffset startedAt,
        IReadOnlyDictionary<string, string> details)
    {
        return new ConnectorExecutionOutcome(
            request.ExecutionId,
            status,
            code,
            message,
            attempts,
            startedAt,
            DateTimeOffset.UtcNow,
            Redact(details));
    }

    private static IReadOnlyDictionary<string, string> Redact(IReadOnlyDictionary<string, string> details)
    {
        return details.ToDictionary(
            static item => item.Key,
            static item => SensitiveParameterNames.Contains(item.Key) ? "[REDACTED]" : item.Value,
            StringComparer.OrdinalIgnoreCase);
    }
}
