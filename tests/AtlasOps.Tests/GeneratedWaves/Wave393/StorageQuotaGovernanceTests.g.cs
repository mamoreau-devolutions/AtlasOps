namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Storage.StorageQuotaGovernance;

[TestClass]
public sealed class StorageQuotaGovernanceTests
{
    private static UpdateStorageQuotaGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("storage.storagequotagovernance-1", "Storage Quota Governance", "Operations", targetState, 3, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "storage.storagequotagovernance");

        Assert.AreEqual(393, descriptor.Wave);
        Assert.AreEqual("Storage", descriptor.Area);
        Assert.AreEqual(typeof(StorageQuotaGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        StorageQuotaGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        StorageQuotaGovernanceValidator validator = new();
        UpdateStorageQuotaGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        StorageQuotaGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<StorageQuotaGovernanceItem> repository = new();
        StorageQuotaGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<StorageQuotaGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        StorageQuotaGovernanceItem? stored = await repository.GetAsync("storage.storagequotagovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<StorageQuotaGovernanceItem> repository = new();
        StorageQuotaGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<StorageQuotaGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        StorageQuotaGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}