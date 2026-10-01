namespace AtlasOps.DesignSystem;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record DesignSystemDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record DesignSystemOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DesignSystemOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IDesignSystemPipeline { ValueTask<DesignSystemOutcome> ExecuteAsync(DesignSystemOperation operation, CancellationToken cancellationToken); }

public sealed class DesignSystemRegistry
{
    private readonly ConcurrentDictionary<string, DesignSystemDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<DesignSystemDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(DesignSystemDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out DesignSystemDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<DesignSystemDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class DesignSystemPipeline : IDesignSystemPipeline
{
    private readonly List<Func<DesignSystemOperation, CancellationToken, ValueTask<DesignSystemOutcome?>>> _stages = new();
    public void Add(Func<DesignSystemOperation, CancellationToken, ValueTask<DesignSystemOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<DesignSystemOutcome> ExecuteAsync(DesignSystemOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<DesignSystemOperation, CancellationToken, ValueTask<DesignSystemOutcome?>> stage in _stages)
        {
            DesignSystemOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new DesignSystemOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}