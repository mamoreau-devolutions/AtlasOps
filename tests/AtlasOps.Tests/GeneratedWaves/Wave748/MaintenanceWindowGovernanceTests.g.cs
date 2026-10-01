namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.ServiceManagement.MaintenanceWindowGovernance;

[TestClass]
public sealed class MaintenanceWindowGovernanceTests
{
    private static UpdateMaintenanceWindowGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("servicemanagement.maintenancewindowgovernance-1", "Maintenance Window Governance", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "servicemanagement.maintenancewindowgovernance");

        Assert.AreEqual(748, descriptor.Wave);
        Assert.AreEqual("ServiceManagement", descriptor.Area);
        Assert.AreEqual(typeof(MaintenanceWindowGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MaintenanceWindowGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MaintenanceWindowGovernanceValidator validator = new();
        UpdateMaintenanceWindowGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MaintenanceWindowGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MaintenanceWindowGovernanceItem> repository = new();
        MaintenanceWindowGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MaintenanceWindowGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MaintenanceWindowGovernanceItem? stored = await repository.GetAsync("servicemanagement.maintenancewindowgovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MaintenanceWindowGovernanceItem> repository = new();
        MaintenanceWindowGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MaintenanceWindowGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MaintenanceWindowGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}