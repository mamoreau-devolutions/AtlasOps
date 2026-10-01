namespace AtlasOps.Integrations.Tests;

using AtlasOps.Integrations.Databases.Contracts;
using AtlasOps.Integrations.Databases.Core;

[TestClass]
public sealed class DatabasePlanningServiceTests
{
    [TestMethod]
    public void Validate_SqliteAtInclusiveBoundaries_DoesNotRequireNetworkOrTls()
    {
        DatabasePlanningService service = new();
        DatabaseEndpoint endpoint = new(DatabaseProviderKind.Sqlite, "", 0, "local.db", "", false);
        DatabaseQueryPlan plan = new(
            "SELECT 1",
            [],
            TimeSpan.FromMilliseconds(100),
            true,
            1_000_000);

        DatabaseQueryValidation result = service.Validate(endpoint, plan);

        Assert.IsTrue(result.Valid);
        Assert.IsEmpty(result.Diagnostics);

        DatabaseQueryValidation upperTimeout = service.Validate(
            endpoint,
            plan with { Timeout = TimeSpan.FromHours(1), MaximumRows = 1 });
        Assert.IsTrue(upperTimeout.Valid);
    }

    [TestMethod]
    public void Validate_RemoteAndPlanBoundaryViolations_ReturnConcreteDiagnostics()
    {
        DatabasePlanningService service = new();
        DatabaseEndpoint endpoint = new(DatabaseProviderKind.PostgreSql, "", 65_536, "", "credential", false);
        DatabaseQueryPlan plan = new("", [], TimeSpan.FromMilliseconds(99), false, 0);

        DatabaseQueryValidation result = service.Validate(endpoint, plan);

        Assert.HasCount(6, result.Diagnostics);
        Assert.Contains("Network database providers require a valid host and port.", result.Diagnostics);
        Assert.Contains("Remote database connections must require TLS.", result.Diagnostics);
        Assert.Contains("Maximum rows must be between 1 and 1,000,000.", result.Diagnostics);
    }

    [TestMethod]
    public void Validate_MissingParameterAndReadOnlyMutation_AreRejected()
    {
        DatabasePlanningService service = new();
        DatabaseEndpoint endpoint = new(DatabaseProviderKind.SqlServer, "host", 1433, "db", "credential", true);
        DatabaseQueryPlan plan = new(
            "UPDATE Items SET Name = @name WHERE Id = :id",
            [new DatabaseParameter(":NAME", "value", "string", false)],
            TimeSpan.FromSeconds(1),
            true,
            100);

        DatabaseQueryValidation result = service.Validate(endpoint, plan);

        CollectionAssert.AreEqual(
            new[]
            {
                "Parameter 'id' is referenced but not declared.",
                "Read-only query plans cannot contain mutation statements.",
            },
            result.Diagnostics.ToArray());
    }

    [TestMethod]
    public void CreateParameterMap_NormalizesPrefixesWhitespaceAndCaseInsensitiveDuplicates()
    {
        DatabasePlanningService service = new();
        DatabaseQueryPlan plan = new(
            "SELECT 1",
            [
                new DatabaseParameter(" @Name ", "first", "string", false),
                new DatabaseParameter(":name", "second", "string", false),
            ],
            TimeSpan.FromSeconds(1),
            true,
            1);

        IReadOnlyDictionary<string, object?> result = service.CreateParameterMap(plan);

        Assert.HasCount(1, result);
        Assert.AreEqual("second", result["NAME"]);
    }
}
