namespace AtlasOps.Connectors.Runtime;

using System.Collections.Concurrent;

using AtlasOps.Connectors.Contracts;

public sealed class ConnectorRegistry
{
    private readonly ConcurrentDictionary<string, ConnectorDefinition> definitions =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IConnectorHandler> handlers =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, IConnectorHealthProbe> healthProbes =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<ConnectorDefinition> Definitions => this.definitions.Values
        .OrderBy(static item => item.Kind)
        .ThenBy(static item => item.DisplayName, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public bool Register(
        ConnectorDefinition definition,
        IConnectorHandler handler,
        IConnectorHealthProbe? healthProbe = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(handler);
        Validate(definition);

        if (!string.Equals(definition.Id, handler.ConnectorId, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The handler connector ID must match the definition.", nameof(handler));
        }

        if (!this.definitions.TryAdd(definition.Id, definition))
        {
            return false;
        }

        if (!this.handlers.TryAdd(definition.Id, handler))
        {
            this.definitions.TryRemove(definition.Id, out _);
            return false;
        }

        if (healthProbe is not null)
        {
            if (!string.Equals(definition.Id, healthProbe.ConnectorId, StringComparison.OrdinalIgnoreCase))
            {
                this.handlers.TryRemove(definition.Id, out _);
                this.definitions.TryRemove(definition.Id, out _);
                throw new ArgumentException("The health probe connector ID must match the definition.", nameof(healthProbe));
            }

            this.healthProbes.TryAdd(definition.Id, healthProbe);
        }

        return true;
    }

    public bool TryResolve(
        string connectorId,
        out ConnectorDefinition? definition,
        out IConnectorHandler? handler)
    {
        bool foundDefinition = this.definitions.TryGetValue(connectorId, out definition);
        bool foundHandler = this.handlers.TryGetValue(connectorId, out handler);
        return foundDefinition && foundHandler;
    }

    public bool TryResolveHealthProbe(string connectorId, out IConnectorHealthProbe? healthProbe)
    {
        return this.healthProbes.TryGetValue(connectorId, out healthProbe);
    }

    public IReadOnlyList<ConnectorDefinition> Search(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return this.Definitions;
        }

        return this.Definitions
            .Where(item =>
                item.Id.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Kind.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static void Validate(ConnectorDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(definition.Id) || definition.Id.Length > 120)
        {
            throw new ArgumentException("Connector ID must contain between 1 and 120 characters.", nameof(definition));
        }

        if (string.IsNullOrWhiteSpace(definition.DisplayName) || definition.DisplayName.Length > 160)
        {
            throw new ArgumentException("Display name must contain between 1 and 160 characters.", nameof(definition));
        }

        if (definition.MaximumConcurrency is < 1 or > 1024)
        {
            throw new ArgumentOutOfRangeException(nameof(definition), "Maximum concurrency must be between 1 and 1024.");
        }

        if (definition.RequestsPerMinute is < 1 or > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(definition), "Requests per minute must be between 1 and 1,000,000.");
        }

        if (definition.MaximumAttempts is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(definition), "Maximum attempts must be between 1 and 10.");
        }

        if (definition.Timeout <= TimeSpan.Zero || definition.Timeout > TimeSpan.FromHours(1))
        {
            throw new ArgumentOutOfRangeException(nameof(definition), "Timeout must be positive and no more than one hour.");
        }
    }
}
