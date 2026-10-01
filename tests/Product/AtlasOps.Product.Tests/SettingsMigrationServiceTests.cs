namespace AtlasOps.Product.Tests;

using AtlasOps.Product.Migrations;

[TestClass]
public sealed class SettingsMigrationServiceTests
{
    [TestMethod]
    public void CreateChecksum_IsCanonicalAcrossDictionaryInsertionOrder()
    {
        Dictionary<string, string> first = new() { ["b"] = "2", ["a"] = "1" };
        Dictionary<string, string> second = new() { ["a"] = "1", ["b"] = "2" };

        string firstChecksum = SettingsMigrationService.CreateChecksum(first);
        string secondChecksum = SettingsMigrationService.CreateChecksum(second);

        Assert.AreEqual(firstChecksum, secondChecksum);
        Assert.AreEqual(64, firstChecksum.Length);
    }

    [TestMethod]
    public void Migrate_ContiguousChainAppliesInOrderAndRegeneratesChecksum()
    {
        SettingsMigrationService service = new();
        Dictionary<string, string> values = new() { ["name"] = "old" };
        SettingsSnapshot snapshot = new(1, values, SettingsMigrationService.CreateChecksum(values));
        SettingsMigration[] migrations =
        [
            new("one-two", 1, 2, current => new Dictionary<string, string>(current) { ["name"] = "new" }),
            new("two-three", 2, 3, current => new Dictionary<string, string>(current) { ["added"] = "yes" }),
        ];

        SettingsMigrationResult result = service.Migrate(snapshot, 3, migrations);

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(3, result.Snapshot.Version);
        Assert.AreEqual("new", result.Snapshot.Values["name"]);
        Assert.AreEqual("yes", result.Snapshot.Values["added"]);
        CollectionAssert.AreEqual(new[] { "one-two", "two-three" }, result.AppliedMigrations.ToArray());
        Assert.AreEqual(
            SettingsMigrationService.CreateChecksum(result.Snapshot.Values),
            result.Snapshot.Checksum);
    }

    [TestMethod]
    public void Migrate_InvalidChecksumRejectsBeforeApplyingMigration()
    {
        SettingsMigrationService service = new();
        SettingsSnapshot snapshot = new(1, new Dictionary<string, string> { ["a"] = "1" }, "BAD");

        SettingsMigrationResult result = service.Migrate(snapshot, 2, []);

        Assert.IsFalse(result.Succeeded);
        Assert.AreSame(snapshot, result.Snapshot);
        Assert.IsEmpty(result.AppliedMigrations);
        Assert.AreEqual("Settings checksum does not match the supplied values.", result.Diagnostics.Single());
    }

    [TestMethod]
    public void Migrate_MissingOrNonContiguousStepReturnsPartialEvidence()
    {
        SettingsMigrationService service = new();
        Dictionary<string, string> values = new() { ["a"] = "1" };
        SettingsSnapshot snapshot = new(1, values, SettingsMigrationService.CreateChecksum(values));
        SettingsMigration invalid = new("skip", 1, 3, static current => current);

        SettingsMigrationResult result = service.Migrate(snapshot, 3, [invalid]);

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(1, result.Snapshot.Version);
        Assert.IsEmpty(result.AppliedMigrations);
        Assert.AreEqual("No contiguous migration starts at version 1.", result.Diagnostics.Single());
    }

    [TestMethod]
    public void Migrate_TargetAlreadyReached_IsSuccessfulNoOp()
    {
        SettingsMigrationService service = new();
        Dictionary<string, string> values = new() { ["a"] = "1" };
        SettingsSnapshot snapshot = new(2, values, SettingsMigrationService.CreateChecksum(values));

        SettingsMigrationResult result = service.Migrate(snapshot, 2, []);

        Assert.IsTrue(result.Succeeded);
        Assert.AreSame(snapshot, result.Snapshot);
        Assert.IsEmpty(result.AppliedMigrations);
    }
}
