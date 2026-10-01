namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Observability.ObservabilityDashboardProvisioning;

[TestClass]
public sealed class ObservabilityDashboardProvisioningTests
{
    private static UpdateObservabilityDashboardProvisioningCommand CreateCommand(string targetState = "Ready")
    {
        return new("observability.observabilitydashboardprovisioning-1", "Observability Dashboard Provisioning", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "observability.observabilitydashboardprovisioning");

        Assert.AreEqual(641, descriptor.Wave);
        Assert.AreEqual("Observability", descriptor.Area);
        Assert.AreEqual(typeof(ObservabilityDashboardProvisioningItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ObservabilityDashboardProvisioningValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ObservabilityDashboardProvisioningValidator validator = new();
        UpdateObservabilityDashboardProvisioningCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ObservabilityDashboardProvisioningPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ObservabilityDashboardProvisioningItem> repository = new();
        ObservabilityDashboardProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ObservabilityDashboardProvisioningChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ObservabilityDashboardProvisioningItem? stored = await repository.GetAsync("observability.observabilitydashboardprovisioning-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ObservabilityDashboardProvisioningItem> repository = new();
        ObservabilityDashboardProvisioningService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ObservabilityDashboardProvisioningChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ObservabilityDashboardProvisioningViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}