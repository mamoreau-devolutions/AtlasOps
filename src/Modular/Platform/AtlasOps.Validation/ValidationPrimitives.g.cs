namespace AtlasOps.Validation;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed record ValidationDescriptor(string Id, string DisplayName, string Category, int Order, IReadOnlyDictionary<string, string> Metadata);
public sealed record ValidationOperation(string Id, string Kind, string Subject, DateTimeOffset RequestedAt, IReadOnlyDictionary<string, string> Parameters);
public sealed record ValidationOutcome(bool Succeeded, string Code, string Message, TimeSpan Duration, IReadOnlyDictionary<string, string> Details);
public interface IValidationPipeline { ValueTask<ValidationOutcome> ExecuteAsync(ValidationOperation operation, CancellationToken cancellationToken); }

public sealed class ValidationRegistry
{
    private readonly ConcurrentDictionary<string, ValidationDescriptor> _items = new(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<ValidationDescriptor> Items => _items.Values.OrderBy(static item => item.Order).ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase).ToArray();
    public bool Register(ValidationDescriptor descriptor) { ArgumentNullException.ThrowIfNull(descriptor); return _items.TryAdd(descriptor.Id, descriptor); }
    public bool TryGet(string id, out ValidationDescriptor? descriptor) { return _items.TryGetValue(id, out descriptor); }
    public IReadOnlyList<ValidationDescriptor> Find(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Items; }
        return Items.Where(item => item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) || item.Category.Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}

public sealed class ValidationPipeline : IValidationPipeline
{
    private readonly List<Func<ValidationOperation, CancellationToken, ValueTask<ValidationOutcome?>>> _stages = new();
    public void Add(Func<ValidationOperation, CancellationToken, ValueTask<ValidationOutcome?>> stage) { ArgumentNullException.ThrowIfNull(stage); _stages.Add(stage); }
    public async ValueTask<ValidationOutcome> ExecuteAsync(ValidationOperation operation, CancellationToken cancellationToken)
    {
        long started = Environment.TickCount64;
        foreach (Func<ValidationOperation, CancellationToken, ValueTask<ValidationOutcome?>> stage in _stages)
        {
            ValidationOutcome? outcome = await stage(operation, cancellationToken).ConfigureAwait(false);
            if (outcome is not null) { return outcome; }
        }
        return new ValidationOutcome(false, "unhandled", "No pipeline stage handled the operation.", TimeSpan.FromMilliseconds(Environment.TickCount64 - started), new Dictionary<string, string>());
    }
}