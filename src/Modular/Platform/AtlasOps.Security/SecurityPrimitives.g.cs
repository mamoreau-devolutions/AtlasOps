namespace AtlasOps.Security;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record SecurityDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record SecurityOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SecurityOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ISecurityPipeline { ValueTask<SecurityOutcome> ExecuteAsync(SecurityOperation operation, CancellationToken cancellationToken); }

public sealed class SecurityRegistry
{
    private readonly ConcurrentDictionary<string, SecurityDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<SecurityDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(SecurityDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out SecurityDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<SecurityDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class SecurityPipeline : ISecurityPipeline
{
    private readonly List<Func<SecurityOperation, CancellationToken, ValueTask<SecurityOutcome?>>> _stages = new();
    public void Add(Func<SecurityOperation, CancellationToken, ValueTask<SecurityOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<SecurityOutcome> ExecuteAsync(SecurityOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<SecurityOperation, CancellationToken, ValueTask<SecurityOutcome?>> stage in _stages)
        {
            SecurityOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new SecurityOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}