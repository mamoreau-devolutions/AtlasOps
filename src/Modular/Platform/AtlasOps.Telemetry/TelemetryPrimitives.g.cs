namespace AtlasOps.Telemetry;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record TelemetryDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record TelemetryOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record TelemetryOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ITelemetryPipeline { ValueTask<TelemetryOutcome> ExecuteAsync(TelemetryOperation operation, CancellationToken cancellationToken); }

public sealed class TelemetryRegistry
{
    private readonly ConcurrentDictionary<string, TelemetryDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<TelemetryDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(TelemetryDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out TelemetryDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<TelemetryDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class TelemetryPipeline : ITelemetryPipeline
{
    private readonly List<Func<TelemetryOperation, CancellationToken, ValueTask<TelemetryOutcome?>>> _stages = new();
    public void Add(Func<TelemetryOperation, CancellationToken, ValueTask<TelemetryOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<TelemetryOutcome> ExecuteAsync(TelemetryOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<TelemetryOperation, CancellationToken, ValueTask<TelemetryOutcome?>> stage in _stages)
        {
            TelemetryOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new TelemetryOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}