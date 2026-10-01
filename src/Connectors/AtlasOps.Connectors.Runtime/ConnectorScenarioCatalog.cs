namespace AtlasOps.Connectors.Runtime;

using System.Collections.Concurrent;
using System.Diagnostics;

using AtlasOps.Connectors.Contracts;

public static class ConnectorScenarioCatalog
{
    public static ConnectorRegistry CreateRegistry()
    {
        ConnectorRegistry registry = new();
        Register(
            registry,
            "asset-discovery",
            "Asset discovery",
            ConnectorKind.Database,
            ["inventory", "synchronization"],
            TimeSpan.FromMilliseconds(12));
        Register(
            registry,
            "change-intelligence",
            "Change intelligence",
            ConnectorKind.Collaboration,
            ["changes", "pull requests"],
            TimeSpan.FromMilliseconds(18));
        Register(
            registry,
            "service-health",
            "Service health",
            ConnectorKind.Observability,
            ["health", "incidents"],
            TimeSpan.FromMilliseconds(8));
        Register(
            registry,
            "document-exchange",
            "Document exchange",
            ConnectorKind.Document,
            ["reports", "archives"],
            TimeSpan.FromMilliseconds(15));
        Register(
            registry,
            "identity-governance",
            "Identity governance",
            ConnectorKind.Identity,
            ["identities", "access reviews"],
            TimeSpan.FromMilliseconds(20));
        return registry;
    }

    private static void Register(
        ConnectorRegistry registry,
        string id,
        string name,
        ConnectorKind kind,
        IReadOnlyList<string> capabilities,
        TimeSpan latency)
    {
        ConnectorDefinition definition = new(
            id,
            name,
            kind,
            true,
            4,
            600,
            3,
            TimeSpan.FromSeconds(5),
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["capabilities"] = string.Join(",", capabilities),
                ["mode"] = "offline-reference",
            });
        ScenarioConnector connector = new(definition.Id, latency);
        registry.Register(definition, connector, connector);
    }

    private sealed class ScenarioConnector(string connectorId, TimeSpan latency) :
        IConnectorHandler,
        IConnectorHealthProbe
    {
        public string ConnectorId { get; } = connectorId;

        public async ValueTask<ConnectorExecutionOutcome> ExecuteAsync(
            ConnectorExecutionRequest request,
            CancellationToken cancellationToken)
        {
            DateTimeOffset startedAt = DateTimeOffset.UtcNow;
            await Task.Delay(latency, cancellationToken).ConfigureAwait(false);
            Dictionary<string, string> details = new(StringComparer.OrdinalIgnoreCase)
            {
                ["resource"] = request.ResourceId,
                ["operation"] = request.Operation,
                ["changes"] = "1",
            };
            return new ConnectorExecutionOutcome(
                request.ExecutionId,
                ConnectorExecutionStatus.Succeeded,
                "completed",
                "The reference connector completed the operation.",
                1,
                startedAt,
                DateTimeOffset.UtcNow,
                details);
        }

        public async ValueTask<ConnectorHealthSnapshot> CheckAsync(CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await Task.Delay(latency, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            return new ConnectorHealthSnapshot(
                this.ConnectorId,
                ConnectorHealthState.Healthy,
                "Reference connector is ready.",
                DateTimeOffset.UtcNow,
                stopwatch.Elapsed,
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["mode"] = "offline-reference",
                });
        }
    }
}
