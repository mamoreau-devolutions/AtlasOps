namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Edge.EdgeTelemetryMonitoring;

[TestClass]
public sealed class EdgeTelemetryMonitoringTests
{
    private static UpdateEdgeTelemetryMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("edge.edgetelemetrymonitoring-1", "Edge Telemetry Monitoring", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "edge.edgetelemetrymonitoring");

        Assert.AreEqual(892, descriptor.Wave);
        Assert.AreEqual("Edge", descriptor.Area);
        Assert.AreEqual(typeof(EdgeTelemetryMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        EdgeTelemetryMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        EdgeTelemetryMonitoringValidator validator = new();
        UpdateEdgeTelemetryMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        EdgeTelemetryMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<EdgeTelemetryMonitoringItem> repository = new();
        EdgeTelemetryMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<EdgeTelemetryMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        EdgeTelemetryMonitoringItem? stored = await repository.GetAsync("edge.edgetelemetrymonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<EdgeTelemetryMonitoringItem> repository = new();
        EdgeTelemetryMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<EdgeTelemetryMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        EdgeTelemetryMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}