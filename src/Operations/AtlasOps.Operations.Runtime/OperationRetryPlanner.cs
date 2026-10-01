namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public sealed record RetryDecision(
    bool Retry,
    bool DeadLetter,
    DateTimeOffset AvailableAt,
    string Diagnostic);

public sealed class OperationRetryPlanner
{
    private readonly TimeSpan minimumDelay;
    private readonly TimeSpan maximumDelay;

    public OperationRetryPlanner(TimeSpan minimumDelay, TimeSpan maximumDelay)
    {
        this.minimumDelay = minimumDelay;
        this.maximumDelay = maximumDelay;
    }

    public RetryDecision Plan(
        DurableJob job,
        OperationExecutionResult result,
        DateTimeOffset now,
        TimeSpan? retryAfter)
    {
        if (result.Succeeded)
        {
            return new RetryDecision(false, false, now, string.Empty);
        }

        if (result.FailureKind is OperationFailureKind.Permanent
            or OperationFailureKind.Validation
            or OperationFailureKind.Authentication
            or OperationFailureKind.Authorization)
        {
            return new RetryDecision(false, true, now, result.Diagnostic);
        }

        if (job.Attempt >= job.MaximumAttempts)
        {
            return new RetryDecision(false, true, now, "Maximum attempts have been exhausted.");
        }

        double multiplier = Math.Pow(2d, Math.Max(0, job.Attempt - 1));
        double requestedTicks = this.minimumDelay.Ticks * multiplier;
        long boundedTicks = (long)Math.Min(this.maximumDelay.Ticks, requestedTicks);
        TimeSpan computed = TimeSpan.FromTicks(boundedTicks);
        TimeSpan delay = retryAfter is not null && retryAfter > computed
            ? retryAfter.Value
            : computed;

        if (delay > this.maximumDelay)
        {
            delay = this.maximumDelay;
        }

        return new RetryDecision(true, false, now.Add(delay), result.Diagnostic);
    }
}
