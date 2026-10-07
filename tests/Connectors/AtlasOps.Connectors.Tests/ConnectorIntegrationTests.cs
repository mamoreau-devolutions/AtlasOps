namespace AtlasOps.Connectors.Tests;

using System.Text;

using AtlasOps.Connectors.Collaboration;
using AtlasOps.Connectors.Data;
using AtlasOps.Connectors.Documents;
using AtlasOps.Connectors.Http;
using AtlasOps.Connectors.Observability;
using AtlasOps.Connectors.Runtime;
using AtlasOps.Connectors.Security;

using RestSharp;

[TestClass]
public sealed class ConnectorIntegrationTests
{
    [TestMethod]
    public void HttpPlanner_RequestAndPreview_ProduceDeterministicOutput()
    {
        HttpRequestPlanner planner = new();
        Uri endpoint = planner.BuildEndpoint(
            new Uri("https://example.test/api/"),
            "resources",
            new Dictionary<string, string>
            {
                ["page"] = "2",
            });
        HttpRequestPlan plan = new(
            endpoint,
            Method.Get,
            new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer secret",
            },
            new Dictionary<string, string>(),
            null);

        RestRequest request = planner.CreateRequest(plan);
        string preview = planner.ExtractTextPreview("<p>Hello <strong>AtlasOps</strong></p>", 100);
        IReadOnlyDictionary<string, string> diagnostics = planner.CreateDiagnosticHeaders(plan.Headers);

        Assert.AreEqual(Method.Get, request.Method);
        Assert.AreEqual("Hello AtlasOps", preview);
        Assert.AreEqual("[REDACTED]", diagnostics["Authorization"]);
        StringAssert.Contains(endpoint.Query, "page=2");
    }

    [TestMethod]
    public void HttpPlanner_JsonSchema_ReportsInvalidPayload()
    {
        HttpRequestPlanner planner = new();
        const string schema = """{"type":"object","required":["id"],"properties":{"id":{"type":"string"}}}""";

        JsonValidationResult valid = planner.ValidateJson("""{"id":"atlasops"}""", schema);
        JsonValidationResult invalid = planner.ValidateJson("""{"id":12}""", schema);

        Assert.IsTrue(valid.Valid);
        Assert.IsFalse(invalid.Valid);
        Assert.IsNotEmpty(invalid.Diagnostics);
    }

    [TestMethod]
    public async Task DataStore_InMemorySqlite_CreatesConnectorSchema()
    {
        await using ConnectorDataStore store = new("Data Source=:memory:");

        bool created = await store.EnsureCreatedAsync();
        bool createdAgain = await store.EnsureCreatedAsync();
        await store.AddStateAsync(
            new ConnectorStateEntity
            {
                Id = Guid.NewGuid(),
                ConnectorId = "test",
                DisplayName = "Test",
                ConfigurationJson = "{}",
                Enabled = true,
                Revision = 1,
                UpdatedAt = DateTimeOffset.UtcNow,
            });

        Assert.IsTrue(created);
        Assert.IsFalse(createdAgain);
        Assert.AreEqual(1, await store.CountStatesAsync());
        Assert.AreEqual("test", Assert.ContainsSingle(await store.ReadStatesAsync()).ConnectorId);
    }

    [TestMethod]
    public async Task DataStore_Checkpoints_UseOptimisticRevisions()
    {
        await using ConnectorDataStore store = new("Data Source=:memory:");
        await store.EnsureCreatedAsync();
        DateTimeOffset now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

        bool inserted = await store.TrySaveCheckpointAsync("test", "default", "cursor-1", 0, now);
        bool duplicateInsert = await store.TrySaveCheckpointAsync("test", "default", "cursor-x", 0, now);
        bool updated = await store.TrySaveCheckpointAsync("test", "default", "cursor-2", 1, now.AddMinutes(1));
        bool staleUpdate = await store.TrySaveCheckpointAsync("test", "default", "cursor-y", 1, now.AddMinutes(2));
        ConnectorCheckpointEntity? checkpoint = await store.ReadCheckpointAsync("test", "default");

        Assert.IsTrue(inserted);
        Assert.IsFalse(duplicateInsert);
        Assert.IsTrue(updated);
        Assert.IsFalse(staleUpdate);
        Assert.IsNotNull(checkpoint);
        Assert.AreEqual("cursor-2", checkpoint.Cursor);
        Assert.AreEqual(2, checkpoint.Revision);
        Assert.AreEqual(now.AddMinutes(1), checkpoint.UpdatedAt);
    }

    [TestMethod]
    public async Task DataStore_Outbox_ReturnsOnlyPendingEntriesInOrder()
    {
        await using ConnectorDataStore store = new("Data Source=:memory:");
        await store.EnsureCreatedAsync();
        DateTimeOffset now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
        ConnectorOutboxEntity first = new() { Id = Guid.NewGuid(), EventType = "a", PayloadJson = "{}", OccurredAt = now };
        ConnectorOutboxEntity second = new() { Id = Guid.NewGuid(), EventType = "b", PayloadJson = "{}", OccurredAt = now.AddSeconds(1) };
        await store.AddOutboxAsync(second);
        await store.AddOutboxAsync(first);

        bool processed = await store.MarkOutboxProcessedAsync(first.Id, now.AddMinutes(1));
        bool processedAgain = await store.MarkOutboxProcessedAsync(first.Id, now.AddMinutes(2));
        IReadOnlyList<ConnectorOutboxEntity> pending = await store.ReadPendingOutboxAsync(10);

        Assert.IsTrue(processed);
        Assert.IsFalse(processedAgain);
        Assert.AreEqual(second.Id, Assert.ContainsSingle(pending).Id);
    }

    [TestMethod]
    public void SnapshotCache_SetAndRemove_ControlsAvailability()
    {
        using ConnectorSnapshotCache cache = new();
        cache.Set("connector", "snapshot", "value", TimeSpan.FromMinutes(1));

        bool found = cache.TryGet("connector", "snapshot", out string? value);
        cache.Remove("connector", "snapshot");
        bool foundAfterRemoval = cache.TryGet("connector", "snapshot", out string? _);

        Assert.IsTrue(found);
        Assert.AreEqual("value", value);
        Assert.IsFalse(foundAfterRemoval);
    }

    [TestMethod]
    public async Task SecretDeriver_SameInputs_ReturnsSameDerivedKey()
    {
        ConnectorSecretDeriver deriver = new();
        byte[] secret = Encoding.UTF8.GetBytes("atlasops-test-secret");
        byte[] salt = Enumerable.Range(1, 32).Select(static value => (byte)value).ToArray();
        SecretDerivationOptions options = new(2, 8_192, 1, 32);

        SecretDerivationResult first =
            await deriver.DeriveAsync(secret, salt, options, CancellationToken.None);
        SecretDerivationResult second =
            await deriver.DeriveAsync(secret, salt, options, CancellationToken.None);

        CollectionAssert.AreEqual(first.DerivedKey, second.DerivedKey);
        Assert.AreEqual("Argon2id", first.Algorithm);
    }

    [TestMethod]
    public void DocumentService_ArchiveAndManifest_RoundTrip()
    {
        ConnectorDocumentService service = new();
        ConnectorDocument[] documents =
        [
            new("summary.md", "text/markdown", Encoding.UTF8.GetBytes("# Summary")),
            new("data.json", "application/json", Encoding.UTF8.GetBytes("""{"status":"ready"}""")),
        ];

        byte[] archive = service.CreateArchive(documents);
        IReadOnlyList<ConnectorDocument> restored = service.ReadArchive(archive);
        byte[] manifest = service.CreateManifest(documents);
        IReadOnlyList<string> names = service.ReadManifestNames(manifest);

        Assert.HasCount(2, restored);
        Assert.HasCount(2, names);
        CollectionAssert.AreEquivalent(new[] { "summary.md", "data.json" }, names.ToArray());
        StringAssert.Contains(service.RenderMarkdown("# Summary"), "<h1");
    }

    [TestMethod]
    public void PackageCatalogs_BindDistinctIntegrationTypes()
    {
        Type[] types = RuntimePackageCatalog.Packages.Select(static item => item.PrimaryType)
            .Concat(HttpPackageCatalog.Packages.Select(static item => item.PrimaryType))
            .Concat(ConnectorDataProviderCatalog.Providers.Select(static item => item.ProviderType))
            .Concat(ConnectorSecurityCatalog.Packages.Select(static item => item.PrimaryType))
            .Concat(ObservabilityPackageCatalog.Packages.Select(static item => item.PrimaryType))
            .Concat(DocumentPackageCatalog.Packages.Select(static item => item.PrimaryType))
            .ToArray();

        Assert.IsGreaterThanOrEqualTo(35, types.Distinct().Count());
        Assert.HasCount(7, CollaborationConnectorCatalog.Connectors);
    }

    [TestMethod]
    public void AiPromptValidation_UnsafeValues_ReturnsSpecificDiagnostics()
    {
        AiPromptPlan plan = new(
            "missing",
            string.Empty,
            string.Empty,
            string.Empty,
            3d,
            0,
            new Dictionary<string, string>());

        AiPromptValidation validation = AiConnectorCatalog.Validate(plan);

        Assert.IsFalse(validation.Valid);
        Assert.HasCount(5, validation.Diagnostics);
    }
}
