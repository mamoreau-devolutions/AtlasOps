namespace AtlasOps.Auditing;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record AuditingDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record AuditingOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record AuditingOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IAuditingPipeline { ValueTask<AuditingOutcome> ExecuteAsync(AuditingOperation operation, CancellationToken cancellationToken); }

public sealed class AuditingRegistry
{
    private readonly ConcurrentDictionary<string, AuditingDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<AuditingDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(AuditingDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out AuditingDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<AuditingDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class AuditingPipeline : IAuditingPipeline
{
    private readonly List<Func<AuditingOperation, CancellationToken, ValueTask<AuditingOutcome?>>> _stages = new();
    public void Add(Func<AuditingOperation, CancellationToken, ValueTask<AuditingOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<AuditingOutcome> ExecuteAsync(AuditingOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<AuditingOperation, CancellationToken, ValueTask<AuditingOutcome?>> stage in _stages)
        {
            AuditingOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new AuditingOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}