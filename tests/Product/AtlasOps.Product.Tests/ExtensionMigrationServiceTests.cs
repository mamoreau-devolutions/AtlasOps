namespace AtlasOps.Product.Tests;

using AtlasOps.Extensions.Contracts;
using AtlasOps.Extensions.Runtime;

[TestClass]
public sealed class ExtensionMigrationServiceTests
{
    [TestMethod]
    public void Migrate_ContiguousChainRemovesKeysUpdatesSchemaAndPreservesCaseInsensitiveValues()
    {
        ExtensionMigrationService service = new();
        Dictionary<string, string> values = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Required"] = "value",
            ["obsolete"] = "remove",
        };
        ExtensionConfigurationMigration[] migrations =
        [
            new(1, 2, "one-two", ["required"], ["OBSOLETE"]),
            new(2, 3, "two-three", ["required"], []),
        ];

        ExtensionMigrationResult result = service.Migrate(1, 3, values, migrations);

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(3, result.Version);
        CollectionAssert.AreEqual(new[] { "one-two", "two-three" }, result.AppliedMigrations.ToArray());
        Assert.AreEqual("3", result.Values["schemaVersion"]);
        Assert.IsFalse(result.Values.ContainsKey("obsolete"));
        Assert.AreEqual("value", result.Values["REQUIRED"]);
    }

    [TestMethod]
    public void Migrate_NoOpLeavesValuesAndReportsNoAppliedMigrations()
    {
        ExtensionMigrationService service = new();
        Dictionary<string, string> values = new() { ["key"] = "value" };

        ExtensionMigrationResult result = service.Migrate(2, 2, values, []);

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(2, result.Version);
        Assert.IsEmpty(result.AppliedMigrations);
        Assert.AreEqual("value", result.Values["key"]);
    }

    [TestMethod]
    public void Migrate_MissingStepOrRequiredKey_ReturnsPartialStateAndDiagnostic()
    {
        ExtensionMigrationService service = new();
        ExtensionMigrationResult missingStep = service.Migrate(
            1,
            2,
            new Dictionary<string, string>(),
            []);
        ExtensionMigrationResult missingKey = service.Migrate(
            1,
            2,
            new Dictionary<string, string>(),
            [new ExtensionConfigurationMigration(1, 2, "one-two", ["z", "a"], [])]);

        Assert.IsFalse(missingStep.Succeeded);
        Assert.AreEqual("No migration starts at configuration version 1.", missingStep.Diagnostics.Single());
        Assert.IsFalse(missingKey.Succeeded);
        Assert.AreEqual("Migration 'one-two' requires missing keys: a, z.", missingKey.Diagnostics.Single());
        Assert.AreEqual(1, missingKey.Version);
    }

    [TestMethod]
    public void Migrate_ChainOvershootsTarget_ReturnsCompatibilityFailure()
    {
        ExtensionMigrationService service = new();

        ExtensionMigrationResult result = service.Migrate(
            1,
            2,
            new Dictionary<string, string>(),
            [new ExtensionConfigurationMigration(1, 3, "overshoot", [], [])]);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(3, result.Version);
        Assert.AreEqual("Migration chain passed the requested target version.", result.Diagnostics.Single());
    }

    [TestMethod]
    public void Migrate_CurrentVersionAboveTarget_ReturnsCompatibilityFailureWithoutChanges()
    {
        ExtensionMigrationService service = new();
        Dictionary<string, string> values = new() { ["key"] = "value" };

        ExtensionMigrationResult result = service.Migrate(3, 2, values, []);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(3, result.Version);
        Assert.AreEqual("value", result.Values["key"]);
        Assert.IsEmpty(result.AppliedMigrations);
        Assert.AreEqual("Migration chain passed the requested target version.", result.Diagnostics.Single());
    }
}
