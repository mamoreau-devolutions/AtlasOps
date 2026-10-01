namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.BusinessContinuity.RecoveryExerciseRecovery;

[TestClass]
public sealed class RecoveryExerciseRecoveryTests
{
    private static UpdateRecoveryExerciseRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("businesscontinuity.recoveryexerciserecovery-1", "Recovery Exercise Recovery", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "businesscontinuity.recoveryexerciserecovery");

        Assert.AreEqual(1030, descriptor.Wave);
        Assert.AreEqual("BusinessContinuity", descriptor.Area);
        Assert.AreEqual(typeof(RecoveryExerciseRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        RecoveryExerciseRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        RecoveryExerciseRecoveryValidator validator = new();
        UpdateRecoveryExerciseRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        RecoveryExerciseRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryExerciseRecoveryItem> repository = new();
        RecoveryExerciseRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryExerciseRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        RecoveryExerciseRecoveryItem? stored = await repository.GetAsync("businesscontinuity.recoveryexerciserecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryExerciseRecoveryItem> repository = new();
        RecoveryExerciseRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryExerciseRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        RecoveryExerciseRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}