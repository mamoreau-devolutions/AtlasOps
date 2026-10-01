namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Database.DatabaseReplicaMonitoring;

[TestClass]
public sealed class DatabaseReplicaMonitoringTests
{
    private static UpdateDatabaseReplicaMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("database.databasereplicamonitoring-1", "Database Replica Monitoring", "Operations", targetState, 7, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "database.databasereplicamonitoring");

        Assert.AreEqual(327, descriptor.Wave);
        Assert.AreEqual("Database", descriptor.Area);
        Assert.AreEqual(typeof(DatabaseReplicaMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        DatabaseReplicaMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        DatabaseReplicaMonitoringValidator validator = new();
        UpdateDatabaseReplicaMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        DatabaseReplicaMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DatabaseReplicaMonitoringItem> repository = new();
        DatabaseReplicaMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DatabaseReplicaMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        DatabaseReplicaMonitoringItem? stored = await repository.GetAsync("database.databasereplicamonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DatabaseReplicaMonitoringItem> repository = new();
        DatabaseReplicaMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DatabaseReplicaMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        DatabaseReplicaMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}