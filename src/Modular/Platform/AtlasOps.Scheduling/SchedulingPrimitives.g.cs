namespace AtlasOps.Scheduling;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record SchedulingDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record SchedulingOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SchedulingOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ISchedulingPipeline { ValueTask<SchedulingOutcome> ExecuteAsync(SchedulingOperation operation, CancellationToken cancellationToken); }

public sealed class SchedulingRegistry
{
    private readonly ConcurrentDictionary<string, SchedulingDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<SchedulingDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(SchedulingDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out SchedulingDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<SchedulingDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class SchedulingPipeline : ISchedulingPipeline
{
    private readonly List<Func<SchedulingOperation, CancellationToken, ValueTask<SchedulingOutcome?>>> _stages = new();
    public void Add(Func<SchedulingOperation, CancellationToken, ValueTask<SchedulingOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<SchedulingOutcome> ExecuteAsync(SchedulingOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<SchedulingOperation, CancellationToken, ValueTask<SchedulingOutcome?>> stage in _stages)
        {
            SchedulingOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new SchedulingOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}