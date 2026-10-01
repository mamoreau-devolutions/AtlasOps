namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.BusinessContinuity.RecoveryReviewOptimization;

[TestClass]
public sealed class RecoveryReviewOptimizationTests
{
    private static UpdateRecoveryReviewOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("businesscontinuity.recoveryreviewoptimization-1", "Recovery Review Optimization", "Operations", targetState, 9, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "businesscontinuity.recoveryreviewoptimization");

        Assert.AreEqual(1059, descriptor.Wave);
        Assert.AreEqual("BusinessContinuity", descriptor.Area);
        Assert.AreEqual(typeof(RecoveryReviewOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        RecoveryReviewOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        RecoveryReviewOptimizationValidator validator = new();
        UpdateRecoveryReviewOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        RecoveryReviewOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryReviewOptimizationItem> repository = new();
        RecoveryReviewOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryReviewOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        RecoveryReviewOptimizationItem? stored = await repository.GetAsync("businesscontinuity.recoveryreviewoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryReviewOptimizationItem> repository = new();
        RecoveryReviewOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryReviewOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        RecoveryReviewOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}