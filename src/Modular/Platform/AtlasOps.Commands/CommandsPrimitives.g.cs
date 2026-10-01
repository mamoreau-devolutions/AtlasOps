namespace AtlasOps.Commands;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record CommandsDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record CommandsOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record CommandsOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface ICommandsPipeline { ValueTask<CommandsOutcome> ExecuteAsync(CommandsOperation operation, CancellationToken cancellationToken); }

public sealed class CommandsRegistry
{
    private readonly ConcurrentDictionary<string, CommandsDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<CommandsDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(CommandsDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out CommandsDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<CommandsDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class CommandsPipeline : ICommandsPipeline
{
    private readonly List<Func<CommandsOperation, CancellationToken, ValueTask<CommandsOutcome?>>> _stages = new();
    public void Add(Func<CommandsOperation, CancellationToken, ValueTask<CommandsOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<CommandsOutcome> ExecuteAsync(CommandsOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<CommandsOperation, CancellationToken, ValueTask<CommandsOutcome?>> stage in _stages)
        {
            CommandsOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new CommandsOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}