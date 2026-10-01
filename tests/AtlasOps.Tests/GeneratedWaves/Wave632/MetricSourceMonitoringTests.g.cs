namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Observability.MetricSourceMonitoring;

[TestClass]
public sealed class MetricSourceMonitoringTests
{
    private static UpdateMetricSourceMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("observability.metricsourcemonitoring-1", "Metric Source Monitoring", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "observability.metricsourcemonitoring");

        Assert.AreEqual(632, descriptor.Wave);
        Assert.AreEqual("Observability", descriptor.Area);
        Assert.AreEqual(typeof(MetricSourceMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        MetricSourceMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        MetricSourceMonitoringValidator validator = new();
        UpdateMetricSourceMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        MetricSourceMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MetricSourceMonitoringItem> repository = new();
        MetricSourceMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MetricSourceMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        MetricSourceMonitoringItem? stored = await repository.GetAsync("observability.metricsourcemonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<MetricSourceMonitoringItem> repository = new();
        MetricSourceMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<MetricSourceMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        MetricSourceMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}