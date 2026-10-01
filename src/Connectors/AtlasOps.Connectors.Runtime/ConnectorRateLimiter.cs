namespace AtlasOps.Connectors.Runtime;

using System.Collections.Concurrent;
using System.Diagnostics;

public sealed class ConnectorRateLimiter
{
    private readonly ConcurrentDictionary<string, RateState> states =
        new(StringComparer.OrdinalIgnoreCase);

    public bool TryAcquire(string connectorId, int requestsPerMinute, out TimeSpan retryAfter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectorId);
        ArgumentOutOfRangeException.ThrowIfLessThan(requestsPerMinute, 1);

        RateState state = this.states.GetOrAdd(
            connectorId,
            static (_, capacity) => new RateState(capacity),
            requestsPerMinute);

        return state.TryAcquire(requestsPerMinute, out retryAfter);
    }

    private sealed class RateState(int initialCapacity)
    {
        private readonly object sync = new();
        private double tokens = initialCapacity;
        private long lastTimestamp = Stopwatch.GetTimestamp();

        public bool TryAcquire(int capacity, out TimeSpan retryAfter)
        {
            lock (this.sync)
            {
                long now = Stopwatch.GetTimestamp();
                double elapsedSeconds = (now - this.lastTimestamp) / (double)Stopwatch.Frequency;
                this.lastTimestamp = now;

                double refillPerSecond = capacity / 60d;
                this.tokens = Math.Min(capacity, this.tokens + (elapsedSeconds * refillPerSecond));

                if (this.tokens >= 1d)
                {
                    this.tokens -= 1d;
                    retryAfter = TimeSpan.Zero;
                    return true;
                }

                retryAfter = TimeSpan.FromSeconds((1d - this.tokens) / refillPerSecond);
                return false;
            }
        }
    }
}
