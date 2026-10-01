namespace AtlasOps.Search;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record SearchDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record SearchOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SearchOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ISearchPipeline { ValueTask<SearchOutcome> ExecuteAsync(SearchOperation operation, CancellationToken cancellationToken); }

public sealed class SearchRegistry
{
    private readonly ConcurrentDictionary<string, SearchDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<SearchDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(SearchDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out SearchDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<SearchDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class SearchPipeline : ISearchPipeline
{
    private readonly List<Func<SearchOperation, CancellationToken, ValueTask<SearchOutcome?>>> _stages = new();
    public void Add(Func<SearchOperation, CancellationToken, ValueTask<SearchOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<SearchOutcome> ExecuteAsync(SearchOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<SearchOperation, CancellationToken, ValueTask<SearchOutcome?>> stage in _stages)
        {
            SearchOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new SearchOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}