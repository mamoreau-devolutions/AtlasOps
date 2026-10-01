namespace AtlasOps.Domain;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record DomainDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record DomainOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record DomainOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IDomainPipeline { ValueTask<DomainOutcome> ExecuteAsync(DomainOperation operation, CancellationToken cancellationToken); }

public sealed class DomainRegistry
{
    private readonly ConcurrentDictionary<string, DomainDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<DomainDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(DomainDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out DomainDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<DomainDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class DomainPipeline : IDomainPipeline
{
    private readonly List<Func<DomainOperation, CancellationToken, ValueTask<DomainOutcome?>>> _stages = new();
    public void Add(Func<DomainOperation, CancellationToken, ValueTask<DomainOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<DomainOutcome> ExecuteAsync(DomainOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<DomainOperation, CancellationToken, ValueTask<DomainOutcome?>> stage in _stages)
        {
            DomainOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new DomainOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}