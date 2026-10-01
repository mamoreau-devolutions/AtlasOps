namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Database.DatabaseRestoreOptimization;

[TestClass]
public sealed class DatabaseRestoreOptimizationTests
{
    private static UpdateDatabaseRestoreOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("database.databaserestoreoptimization-1", "Database Restore Optimization", "Operations", targetState, 9, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "database.databaserestoreoptimization");

        Assert.AreEqual(339, descriptor.Wave);
        Assert.AreEqual("Database", descriptor.Area);
        Assert.AreEqual(typeof(DatabaseRestoreOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        DatabaseRestoreOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        DatabaseRestoreOptimizationValidator validator = new();
        UpdateDatabaseRestoreOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        DatabaseRestoreOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DatabaseRestoreOptimizationItem> repository = new();
        DatabaseRestoreOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DatabaseRestoreOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        DatabaseRestoreOptimizationItem? stored = await repository.GetAsync("database.databaserestoreoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DatabaseRestoreOptimizationItem> repository = new();
        DatabaseRestoreOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DatabaseRestoreOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        DatabaseRestoreOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}