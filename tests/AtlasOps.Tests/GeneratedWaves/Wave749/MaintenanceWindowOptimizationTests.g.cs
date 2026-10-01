namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.ServiceManagement.MaintenanceWindowOptimization;

[TestClass]
public sealed class MaintenanceWindowOptimizationTests
{
    private static UpdateMaintenanceWindowOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("servicemanagement.maintenancewindowoptimization-1", "Maintenance Window Optimization", "Operations", targetState, 9, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "servicemanagement.maintenancewindowoptimization");

        Assert.AreEqual(749, descriptor.Wave);
        Assert.AreEqual("ServiceManagement", descriptor.Area);
        Assert.AreEqual(typeof(MaintenanceWindowOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MaintenanceWindowOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MaintenanceWindowOptimizationValidator validator = new();
        UpdateMaintenanceWindowOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MaintenanceWindowOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MaintenanceWindowOptimizationItem> repository = new();
        MaintenanceWindowOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MaintenanceWindowOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MaintenanceWindowOptimizationItem? stored = await repository.GetAsync("servicemanagement.maintenancewindowoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MaintenanceWindowOptimizationItem> repository = new();
        MaintenanceWindowOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MaintenanceWindowOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MaintenanceWindowOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}