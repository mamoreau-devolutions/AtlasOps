namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Architecture.ArchitectureComponentMonitoring;

[TestClass]
public sealed class ArchitectureComponentMonitoringTests
{
    private static UpdateArchitectureComponentMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("architecture.architecturecomponentmonitoring-1", "Architecture Component Monitoring", "Operations", targetState, 7, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "architecture.architecturecomponentmonitoring");

        Assert.AreEqual(1067, descriptor.Wave);
        Assert.AreEqual("Architecture", descriptor.Area);
        Assert.AreEqual(typeof(ArchitectureComponentMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ArchitectureComponentMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ArchitectureComponentMonitoringValidator validator = new();
        UpdateArchitectureComponentMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ArchitectureComponentMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ArchitectureComponentMonitoringItem> repository = new();
        ArchitectureComponentMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ArchitectureComponentMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ArchitectureComponentMonitoringItem? stored = await repository.GetAsync("architecture.architecturecomponentmonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ArchitectureComponentMonitoringItem> repository = new();
        ArchitectureComponentMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ArchitectureComponentMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ArchitectureComponentMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}