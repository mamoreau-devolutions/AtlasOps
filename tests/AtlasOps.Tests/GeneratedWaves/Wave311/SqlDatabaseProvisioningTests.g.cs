namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Database.SqlDatabaseProvisioning;

[TestClass]
public sealed class SqlDatabaseProvisioningTests
{
    private static UpdateSqlDatabaseProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("database.sqldatabaseprovisioning-1", "Sql Database Provisioning", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "database.sqldatabaseprovisioning");

        Assert.AreEqual(311, descriptor.Wave);
        Assert.AreEqual("Database", descriptor.Area);
        Assert.AreEqual(typeof(SqlDatabaseProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        SqlDatabaseProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        SqlDatabaseProvisioningValidator validator = new();
        UpdateSqlDatabaseProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        SqlDatabaseProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SqlDatabaseProvisioningItem> repository = new();
        SqlDatabaseProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SqlDatabaseProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        SqlDatabaseProvisioningItem? stored = await repository.GetAsync("database.sqldatabaseprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SqlDatabaseProvisioningItem> repository = new();
        SqlDatabaseProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SqlDatabaseProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        SqlDatabaseProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}