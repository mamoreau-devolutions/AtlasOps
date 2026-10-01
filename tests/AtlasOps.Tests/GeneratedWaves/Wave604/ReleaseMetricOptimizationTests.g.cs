namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Delivery.ReleaseMetricOptimization;

[TestClass]
public sealed class ReleaseMetricOptimizationTests
{
    private static UpdateReleaseMetricOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("delivery.releasemetricoptimization-1", "Release Metric Optimization", "Operations", targetState, 4, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "delivery.releasemetricoptimization");

        Assert.AreEqual(604, descriptor.Wave);
        Assert.AreEqual("Delivery", descriptor.Area);
        Assert.AreEqual(typeof(ReleaseMetricOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ReleaseMetricOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ReleaseMetricOptimizationValidator validator = new();
        UpdateReleaseMetricOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ReleaseMetricOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ReleaseMetricOptimizationItem> repository = new();
        ReleaseMetricOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ReleaseMetricOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ReleaseMetricOptimizationItem? stored = await repository.GetAsync("delivery.releasemetricoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ReleaseMetricOptimizationItem> repository = new();
        ReleaseMetricOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ReleaseMetricOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ReleaseMetricOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}