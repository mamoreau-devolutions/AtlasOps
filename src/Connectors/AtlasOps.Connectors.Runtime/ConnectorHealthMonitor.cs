namespace AtlasOps.Connectors.Runtime;

using System.Collections.Concurrent;

using AtlasOps.Connectors.Contracts;

public sealed class ConnectorHealthMonitor(ConnectorRegistry registry)
{
    private readonly ConcurrentDictionary<string, ConnectorHealthSnapshot> snapshots =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<ConnectorHealthSnapshot> Snapshots => this.snapshots.Values
        .OrderBy(static item => item.State)
        .ThenBy(static item => item.ConnectorId, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public async ValueTask<ConnectorHealthSnapshot> RefreshAsync(
        string connectorId,
        CancellationToken cancellationToken)
    {
        ConnectorDefinition? definition = registry.Definitions.FirstOrDefault(
            item => string.Equals(item.Id, connectorId, StringComparison.OrdinalIgnoreCase));

        if (definition is null)
        {
            throw new KeyNotFoundException($"Connector '{connectorId}' is not registered.");
        }

        if (!definition.Enabled)
        {
            ConnectorHealthSnapshot disabled = new(
                definition.Id,
                ConnectorHealthState.Disabled,
                "Connector is disabled.",
                DateTimeOffset.UtcNow,
                TimeSpan.Zero,
                ConnectorContract.EmptyDetails);
            this.snapshots[definition.Id] = disabled;
            return disabled;
        }

        if (!registry.TryResolveHealthProbe(definition.Id, out IConnectorHealthProbe? probe) || probe is null)
        {
            ConnectorHealthSnapshot unknown = new(
                definition.Id,
                ConnectorHealthState.Unknown,
                "No health probe is registered.",
                DateTimeOffset.UtcNow,
                TimeSpan.Zero,
                ConnectorContract.EmptyDetails);
            this.snapshots[definition.Id] = unknown;
            return unknown;
        }

        ConnectorHealthSnapshot result = await probe.CheckAsync(cancellationToken).ConfigureAwait(false);
        this.snapshots[definition.Id] = result;
        return result;
    }

    public async ValueTask<IReadOnlyList<ConnectorHealthSnapshot>> RefreshAllAsync(
        CancellationToken cancellationToken)
    {
        List<ConnectorHealthSnapshot> refreshed = new();
        foreach (ConnectorDefinition definition in registry.Definitions)
        {
            refreshed.Add(await this.RefreshAsync(definition.Id, cancellationToken).ConfigureAwait(false));
        }

        return refreshed;
    }
}
