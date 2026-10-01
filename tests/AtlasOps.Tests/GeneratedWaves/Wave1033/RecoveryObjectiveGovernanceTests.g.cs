namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.BusinessContinuity.RecoveryObjectiveGovernance;

[TestClass]
public sealed class RecoveryObjectiveGovernanceTests
{
    private static UpdateRecoveryObjectiveGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("businesscontinuity.recoveryobjectivegovernance-1", "Recovery Objective Governance", "Operations", targetState, 3, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "businesscontinuity.recoveryobjectivegovernance");

        Assert.AreEqual(1033, descriptor.Wave);
        Assert.AreEqual("BusinessContinuity", descriptor.Area);
        Assert.AreEqual(typeof(RecoveryObjectiveGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        RecoveryObjectiveGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        RecoveryObjectiveGovernanceValidator validator = new();
        UpdateRecoveryObjectiveGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        RecoveryObjectiveGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryObjectiveGovernanceItem> repository = new();
        RecoveryObjectiveGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryObjectiveGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        RecoveryObjectiveGovernanceItem? stored = await repository.GetAsync("businesscontinuity.recoveryobjectivegovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryObjectiveGovernanceItem> repository = new();
        RecoveryObjectiveGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryObjectiveGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        RecoveryObjectiveGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}