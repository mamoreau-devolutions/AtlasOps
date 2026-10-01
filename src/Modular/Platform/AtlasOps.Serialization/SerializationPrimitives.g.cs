namespace AtlasOps.Serialization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record SerializationDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record SerializationOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record SerializationOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ISerializationPipeline { ValueTask<SerializationOutcome> ExecuteAsync(SerializationOperation operation, CancellationToken cancellationToken); }

public sealed class SerializationRegistry
{
    private readonly ConcurrentDictionary<string, SerializationDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<SerializationDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(SerializationDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out SerializationDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<SerializationDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class SerializationPipeline : ISerializationPipeline
{
    private readonly List<Func<SerializationOperation, CancellationToken, ValueTask<SerializationOutcome?>>> _stages = new();
    public void Add(Func<SerializationOperation, CancellationToken, ValueTask<SerializationOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<SerializationOutcome> ExecuteAsync(SerializationOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<SerializationOperation, CancellationToken, ValueTask<SerializationOutcome?>> stage in _stages)
        {
            SerializationOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new SerializationOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}