namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Database.DatabaseRestoreProvisioning;

[TestClass]
public sealed class DatabaseRestoreProvisioningTests
{
    private static UpdateDatabaseRestoreProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("database.databaserestoreprovisioning-1", "Database Restore Provisioning", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "database.databaserestoreprovisioning");

        Assert.AreEqual(336, descriptor.Wave);
        Assert.AreEqual("Database", descriptor.Area);
        Assert.AreEqual(typeof(DatabaseRestoreProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        DatabaseRestoreProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        DatabaseRestoreProvisioningValidator validator = new();
        UpdateDatabaseRestoreProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        DatabaseRestoreProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DatabaseRestoreProvisioningItem> repository = new();
        DatabaseRestoreProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DatabaseRestoreProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        DatabaseRestoreProvisioningItem? stored = await repository.GetAsync("database.databaserestoreprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DatabaseRestoreProvisioningItem> repository = new();
        DatabaseRestoreProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DatabaseRestoreProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        DatabaseRestoreProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}