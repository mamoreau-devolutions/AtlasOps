namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Sync.DisasterRecovery;

[TestClass]
public sealed class DisasterRecoveryTests
{
    private static UpdateDisasterRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("sync.disasterrecovery-1", "Disaster Recovery", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "sync.disasterrecovery");

        Assert.AreEqual(100, descriptor.Wave);
        Assert.AreEqual("Sync", descriptor.Area);
        Assert.AreEqual(typeof(DisasterRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        DisasterRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        DisasterRecoveryValidator validator = new();
        UpdateDisasterRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        DisasterRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DisasterRecoveryItem> repository = new();
        DisasterRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DisasterRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        DisasterRecoveryItem? stored = await repository.GetAsync("sync.disasterrecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DisasterRecoveryItem> repository = new();
        DisasterRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DisasterRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        DisasterRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}