namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.BusinessContinuity.RecoveryFailoverMonitoring;

[TestClass]
public sealed class RecoveryFailoverMonitoringTests
{
    private static UpdateRecoveryFailoverMonitoringCommand CreateCommand(string targetState = "Ready")
    {
        return new("businesscontinuity.recoveryfailovermonitoring-1", "Recovery Failover Monitoring", "Operations", targetState, 7, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "businesscontinuity.recoveryfailovermonitoring");

        Assert.AreEqual(1047, descriptor.Wave);
        Assert.AreEqual("BusinessContinuity", descriptor.Area);
        Assert.AreEqual(typeof(RecoveryFailoverMonitoringItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        RecoveryFailoverMonitoringValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        RecoveryFailoverMonitoringValidator validator = new();
        UpdateRecoveryFailoverMonitoringCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        RecoveryFailoverMonitoringPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryFailoverMonitoringItem> repository = new();
        RecoveryFailoverMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryFailoverMonitoringChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        RecoveryFailoverMonitoringItem? stored = await repository.GetAsync("businesscontinuity.recoveryfailovermonitoring-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<RecoveryFailoverMonitoringItem> repository = new();
        RecoveryFailoverMonitoringService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<RecoveryFailoverMonitoringChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        RecoveryFailoverMonitoringViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}