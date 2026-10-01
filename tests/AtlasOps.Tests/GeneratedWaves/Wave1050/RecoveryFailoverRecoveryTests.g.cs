namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.BusinessContinuity.RecoveryFailoverRecovery;

[TestClass]
public sealed class RecoveryFailoverRecoveryTests
{
    private static UpdateRecoveryFailoverRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("businesscontinuity.recoveryfailoverrecovery-1", "Recovery Failover Recovery", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "businesscontinuity.recoveryfailoverrecovery");

        Assert.AreEqual(1050, descriptor.Wave);
        Assert.AreEqual("BusinessContinuity", descriptor.Area);
        Assert.AreEqual(typeof(RecoveryFailoverRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        RecoveryFailoverRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        RecoveryFailoverRecoveryValidator validator = new();
        UpdateRecoveryFailoverRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        RecoveryFailoverRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryFailoverRecoveryItem> repository = new();
        RecoveryFailoverRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryFailoverRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        RecoveryFailoverRecoveryItem? stored = await repository.GetAsync("businesscontinuity.recoveryfailoverrecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryFailoverRecoveryItem> repository = new();
        RecoveryFailoverRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryFailoverRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        RecoveryFailoverRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}