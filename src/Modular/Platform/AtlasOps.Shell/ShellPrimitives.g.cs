namespace AtlasOps.Shell;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record ShellDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record ShellOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ShellOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IShellPipeline { ValueTask<ShellOutcome> ExecuteAsync(ShellOperation operation, CancellationToken cancellationToken); }

public sealed class ShellRegistry
{
    private readonly ConcurrentDictionary<string, ShellDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<ShellDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(ShellDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out ShellDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<ShellDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class ShellPipeline : IShellPipeline
{
    private readonly List<Func<ShellOperation, CancellationToken, ValueTask<ShellOutcome?>>> _stages = new();
    public void Add(Func<ShellOperation, CancellationToken, ValueTask<ShellOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<ShellOutcome> ExecuteAsync(ShellOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<ShellOperation, CancellationToken, ValueTask<ShellOutcome?>> stage in _stages)
        {
            ShellOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new ShellOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}