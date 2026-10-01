namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Analytics.AnalyticsSubscription;

[TestClass]
public sealed class AnalyticsSubscriptionTests
{
    private static UpdateAnalyticsSubscriptionCommand CreateCommand(string targetState = "Ready")
    {
        return new("analytics.analyticssubscription-1", "Analytics Subscription", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "analytics.analyticssubscription");

        Assert.AreEqual(80, descriptor.Wave);
        Assert.AreEqual("Analytics", descriptor.Area);
        Assert.AreEqual(typeof(AnalyticsSubscriptionItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        AnalyticsSubscriptionValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        AnalyticsSubscriptionValidator validator = new();
        UpdateAnalyticsSubscriptionCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        AnalyticsSubscriptionPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<AnalyticsSubscriptionItem> repository = new();
        AnalyticsSubscriptionService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<AnalyticsSubscriptionChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        AnalyticsSubscriptionItem? stored = await repository.GetAsync("analytics.analyticssubscription-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<AnalyticsSubscriptionItem> repository = new();
        AnalyticsSubscriptionService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<AnalyticsSubscriptionChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        AnalyticsSubscriptionViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}