namespace AtlasOps.Connectors.Tests;

using AtlasOps.Connectors.Contracts;
using AtlasOps.Connectors.Runtime;

[TestClass]
public sealed class ConnectorRuntimeTests
{
    [TestMethod]
    public async Task ExecuteAsync_RegisteredConnector_PublishesSuccessfulAudit()
    {
        ConnectorRegistry registry = ConnectorScenarioCatalog.CreateRegistry();
        InMemoryConnectorAuditSink audit = new();
        ConnectorRuntime runtime = new(registry, new ConnectorRateLimiter(), audit);
        ConnectorExecutionRequest request = CreateRequest("asset-discovery");

        ConnectorExecutionOutcome outcome = await runtime.ExecuteAsync(request, CancellationToken.None);

        Assert.AreEqual(ConnectorExecutionStatus.Succeeded, outcome.Status);
        Assert.AreEqual(1, outcome.Attempts);
        Assert.HasCount(1, audit.Records);
        Assert.AreEqual(request.ExecutionId, audit.Records[0].ExecutionId);
    }

    [TestMethod]
    public async Task ExecuteAsync_UnknownConnector_ReturnsExplicitNotFound()
    {
        ConnectorRegistry registry = ConnectorScenarioCatalog.CreateRegistry();
        InMemoryConnectorAuditSink audit = new();
        ConnectorRuntime runtime = new(registry, new ConnectorRateLimiter(), audit);

        ConnectorExecutionOutcome outcome = await runtime.ExecuteAsync(
            CreateRequest("missing"),
            CancellationToken.None);

        Assert.AreEqual(ConnectorExecutionStatus.NotFound, outcome.Status);
        Assert.AreEqual("connector-not-found", outcome.Code);
        Assert.HasCount(1, audit.Records);
    }

    [TestMethod]
    public async Task ExecuteAsync_SecretDetail_RedactsAuditValue()
    {
        ConnectorRegistry registry = new();
        CapturingHandler handler = new("secret-test");
        registry.Register(CreateDefinition("secret-test", 10), handler);
        InMemoryConnectorAuditSink audit = new();
        ConnectorRuntime runtime = new(registry, new ConnectorRateLimiter(), audit);

        await runtime.ExecuteAsync(CreateRequest("secret-test"), CancellationToken.None);

        Assert.AreEqual("[REDACTED]", audit.Records[0].Details["token"]);
    }

    [TestMethod]
    public async Task ExecuteAsync_RateLimitExceeded_ReturnsThrottleOutcome()
    {
        ConnectorRegistry registry = new();
        CapturingHandler handler = new("limited");
        registry.Register(CreateDefinition("limited", 1), handler);
        ConnectorRuntime runtime = new(registry, new ConnectorRateLimiter(), new InMemoryConnectorAuditSink());

        ConnectorExecutionOutcome first = await runtime.ExecuteAsync(CreateRequest("limited"), CancellationToken.None);
        ConnectorExecutionOutcome second = await runtime.ExecuteAsync(CreateRequest("limited"), CancellationToken.None);

        Assert.AreEqual(ConnectorExecutionStatus.Succeeded, first.Status);
        Assert.AreEqual(ConnectorExecutionStatus.Throttled, second.Status);
        Assert.AreEqual(1, handler.ExecutionCount);
    }

    [TestMethod]
    public async Task CheckpointStore_StaleRevision_RejectsUpdate()
    {
        InMemoryConnectorCheckpointStore store = new();
        ConnectorCheckpoint first = new(
            "connector",
            "default",
            "one",
            1,
            DateTimeOffset.UtcNow,
            ConnectorContract.EmptyDetails);
        ConnectorCheckpoint stale = first with
        {
            Cursor = "stale",
            Revision = 2,
        };

        bool created = await store.SaveAsync(first, 0, CancellationToken.None);
        bool updated = await store.SaveAsync(stale, 0, CancellationToken.None);

        Assert.IsTrue(created);
        Assert.IsFalse(updated);
        ConnectorCheckpoint? stored = await store.GetAsync("connector", "default", CancellationToken.None);
        Assert.IsNotNull(stored);
        Assert.AreEqual("one", stored.Cursor);
    }

    [TestMethod]
    public async Task HealthMonitor_ReferenceConnectors_ReturnsHealthySnapshots()
    {
        ConnectorRegistry registry = ConnectorScenarioCatalog.CreateRegistry();
        ConnectorHealthMonitor monitor = new(registry);

        IReadOnlyList<ConnectorHealthSnapshot> snapshots =
            await monitor.RefreshAllAsync(CancellationToken.None);

        Assert.HasCount(5, snapshots);
        Assert.IsTrue(snapshots.All(static item => item.State == ConnectorHealthState.Healthy));
    }

    private static ConnectorExecutionRequest CreateRequest(string connectorId)
    {
        return new ConnectorExecutionRequest(
            Guid.NewGuid(),
            connectorId,
            "synchronize",
            "test",
            null,
            DateTimeOffset.UtcNow,
            ConnectorContract.EmptyDetails);
    }

    private static ConnectorDefinition CreateDefinition(string id, int requestsPerMinute)
    {
        return new ConnectorDefinition(
            id,
            id,
            ConnectorKind.Custom,
            true,
            1,
            requestsPerMinute,
            1,
            TimeSpan.FromSeconds(2),
            ConnectorContract.EmptyDetails);
    }

    private sealed class CapturingHandler(string connectorId) : IConnectorHandler
    {
        public string ConnectorId { get; } = connectorId;

        public int ExecutionCount { get; private set; }

        public ValueTask<ConnectorExecutionOutcome> ExecuteAsync(
            ConnectorExecutionRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ExecutionCount++;
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return ValueTask.FromResult(
                new ConnectorExecutionOutcome(
                    request.ExecutionId,
                    ConnectorExecutionStatus.Succeeded,
                    "ok",
                    "done",
                    1,
                    now,
                    now,
                    new Dictionary<string, string>
                    {
                        ["token"] = "secret-value",
                    }));
        }
    }
}
