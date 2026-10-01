namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Analytics.TimelineWidget;

[TestClass]
public sealed class TimelineWidgetTests
{
    private static UpdateTimelineWidgetCommand CreateCommand(string targetState = "Ready")
    {
        return new("analytics.timelinewidget-1", "Timeline Widget", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "analytics.timelinewidget");

        Assert.AreEqual(72, descriptor.Wave);
        Assert.AreEqual("Analytics", descriptor.Area);
        Assert.AreEqual(typeof(TimelineWidgetItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        TimelineWidgetValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        TimelineWidgetValidator validator = new();
        UpdateTimelineWidgetCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        TimelineWidgetPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<TimelineWidgetItem> repository = new();
        TimelineWidgetService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<TimelineWidgetChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        TimelineWidgetItem? stored = await repository.GetAsync("analytics.timelinewidget-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<TimelineWidgetItem> repository = new();
        TimelineWidgetService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<TimelineWidgetChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        TimelineWidgetViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}