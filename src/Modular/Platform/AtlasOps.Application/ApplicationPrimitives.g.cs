namespace AtlasOps.Application;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record ApplicationDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record ApplicationOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ApplicationOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IApplicationPipeline { ValueTask<ApplicationOutcome> ExecuteAsync(ApplicationOperation operation, CancellationToken cancellationToken); }

public sealed class ApplicationRegistry
{
    private readonly ConcurrentDictionary<string, ApplicationDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<ApplicationDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(ApplicationDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out ApplicationDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<ApplicationDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class ApplicationPipeline : IApplicationPipeline
{
    private readonly List<Func<ApplicationOperation, CancellationToken, ValueTask<ApplicationOutcome?>>> _stages = new();
    public void Add(Func<ApplicationOperation, CancellationToken, ValueTask<ApplicationOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<ApplicationOutcome> ExecuteAsync(ApplicationOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<ApplicationOperation, CancellationToken, ValueTask<ApplicationOutcome?>> stage in _stages)
        {
            ApplicationOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new ApplicationOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}