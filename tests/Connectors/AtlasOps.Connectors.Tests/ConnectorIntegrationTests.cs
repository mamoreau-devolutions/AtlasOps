namespace AtlasOps.Connectors.Tests;

using System.Text;

using AtlasOps.Connectors.Collaboration;
using AtlasOps.Connectors.Data;
using AtlasOps.Connectors.Documents;
using AtlasOps.Connectors.Http;
using AtlasOps.Connectors.Observability;
using AtlasOps.Connectors.Runtime;
using AtlasOps.Connectors.Security;

using Microsoft.EntityFrameworkCore;

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
    public async Task DataContext_InMemorySqlite_CreatesConnectorSchema()
    {
        DbContextOptions<ConnectorDataContext> options =
            new DbContextOptionsBuilder<ConnectorDataContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;
        await using ConnectorDataContext context = new(options);
        await context.Database.OpenConnectionAsync();

        bool created = await context.Database.EnsureCreatedAsync();
        context.ConnectorStates.Add(
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
        await context.SaveChangesAsync();

        Assert.IsTrue(created);
        Assert.AreEqual(1, await context.ConnectorStates.CountAsync());
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
