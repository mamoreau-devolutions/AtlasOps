namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Analytics.GaugeWidget;

[TestClass]
public sealed class GaugeWidgetTests
{
    private static UpdateGaugeWidgetCommand CreateCommand(string targetState = "Ready")
    {
        return new("analytics.gaugewidget-1", "Gauge Widget", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "analytics.gaugewidget");

        Assert.AreEqual(71, descriptor.Wave);
        Assert.AreEqual("Analytics", descriptor.Area);
        Assert.AreEqual(typeof(GaugeWidgetItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        GaugeWidgetValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        GaugeWidgetValidator validator = new();
        UpdateGaugeWidgetCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        GaugeWidgetPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<GaugeWidgetItem> repository = new();
        GaugeWidgetService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<GaugeWidgetChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        GaugeWidgetItem? stored = await repository.GetAsync("analytics.gaugewidget-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<GaugeWidgetItem> repository = new();
        GaugeWidgetService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<GaugeWidgetChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        GaugeWidgetViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}