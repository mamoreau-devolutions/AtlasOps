namespace AtlasOps.Configuration;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record ConfigurationDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record ConfigurationOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ConfigurationOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IConfigurationPipeline { ValueTask<ConfigurationOutcome> ExecuteAsync(ConfigurationOperation operation, CancellationToken cancellationToken); }

public sealed class ConfigurationRegistry
{
    private readonly ConcurrentDictionary<string, ConfigurationDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<ConfigurationDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(ConfigurationDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out ConfigurationDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<ConfigurationDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class ConfigurationPipeline : IConfigurationPipeline
{
    private readonly List<Func<ConfigurationOperation, CancellationToken, ValueTask<ConfigurationOutcome?>>> _stages = new();
    public void Add(Func<ConfigurationOperation, CancellationToken, ValueTask<ConfigurationOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<ConfigurationOutcome> ExecuteAsync(ConfigurationOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<ConfigurationOperation, CancellationToken, ValueTask<ConfigurationOutcome?>> stage in _stages)
        {
            ConfigurationOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new ConfigurationOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}