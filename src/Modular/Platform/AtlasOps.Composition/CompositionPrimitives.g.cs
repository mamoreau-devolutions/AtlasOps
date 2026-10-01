namespace AtlasOps.Composition;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record CompositionDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record CompositionOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CompositionOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ICompositionPipeline { ValueTask<CompositionOutcome> ExecuteAsync(CompositionOperation operation, CancellationToken cancellationToken); }

public sealed class CompositionRegistry
{
    private readonly ConcurrentDictionary<string, CompositionDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<CompositionDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(CompositionDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out CompositionDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<CompositionDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class CompositionPipeline : ICompositionPipeline
{
    private readonly List<Func<CompositionOperation, CancellationToken, ValueTask<CompositionOutcome?>>> _stages = new();
    public void Add(Func<CompositionOperation, CancellationToken, ValueTask<CompositionOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<CompositionOutcome> ExecuteAsync(CompositionOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<CompositionOperation, CancellationToken, ValueTask<CompositionOutcome?>> stage in _stages)
        {
            CompositionOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new CompositionOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}