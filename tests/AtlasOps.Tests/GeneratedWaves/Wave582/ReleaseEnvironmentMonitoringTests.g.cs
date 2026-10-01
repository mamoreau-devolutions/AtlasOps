namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Delivery.ReleaseEnvironmentMonitoring;

[TestClass]
public sealed class ReleaseEnvironmentMonitoringTests
{
    private static UpdateReleaseEnvironmentMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("delivery.releaseenvironmentmonitoring-1", "Release Environment Monitoring", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "delivery.releaseenvironmentmonitoring");

        Assert.AreEqual(582, descriptor.Wave);
        Assert.AreEqual("Delivery", descriptor.Area);
        Assert.AreEqual(typeof(ReleaseEnvironmentMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ReleaseEnvironmentMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ReleaseEnvironmentMonitoringValidator validator = new();
        UpdateReleaseEnvironmentMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ReleaseEnvironmentMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ReleaseEnvironmentMonitoringItem> repository = new();
        ReleaseEnvironmentMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ReleaseEnvironmentMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ReleaseEnvironmentMonitoringItem? stored = await repository.GetAsync("delivery.releaseenvironmentmonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ReleaseEnvironmentMonitoringItem> repository = new();
        ReleaseEnvironmentMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ReleaseEnvironmentMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ReleaseEnvironmentMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}