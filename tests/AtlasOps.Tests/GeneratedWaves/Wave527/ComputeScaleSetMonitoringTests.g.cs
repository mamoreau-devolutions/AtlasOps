namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Compute.ComputeScaleSetMonitoring;

[TestClass]
public sealed class ComputeScaleSetMonitoringTests
{
    private static UpdateComputeScaleSetMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("compute.computescalesetmonitoring-1", "Compute Scale Set Monitoring", "Operations", targetState, 7, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "compute.computescalesetmonitoring");

        Assert.AreEqual(527, descriptor.Wave);
        Assert.AreEqual("Compute", descriptor.Area);
        Assert.AreEqual(typeof(ComputeScaleSetMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ComputeScaleSetMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ComputeScaleSetMonitoringValidator validator = new();
        UpdateComputeScaleSetMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ComputeScaleSetMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ComputeScaleSetMonitoringItem> repository = new();
        ComputeScaleSetMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ComputeScaleSetMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ComputeScaleSetMonitoringItem? stored = await repository.GetAsync("compute.computescalesetmonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ComputeScaleSetMonitoringItem> repository = new();
        ComputeScaleSetMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ComputeScaleSetMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ComputeScaleSetMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}