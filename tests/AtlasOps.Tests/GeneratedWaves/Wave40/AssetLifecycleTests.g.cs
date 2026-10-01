namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Inventory.AssetLifecycle;

[TestClass]
public sealed class AssetLifecycleTests
{
    private static UpdateAssetLifecycleCommand CreateCommand(string targetState = "Ready")
    {
        return new("inventory.assetlifecycle-1", "Asset Lifecycle", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "inventory.assetlifecycle");

        Assert.AreEqual(40, descriptor.Wave);
        Assert.AreEqual("Inventory", descriptor.Area);
        Assert.AreEqual(typeof(AssetLifecycleItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        AssetLifecycleValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        AssetLifecycleValidator validator = new();
        UpdateAssetLifecycleCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        AssetLifecyclePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<AssetLifecycleItem> repository = new();
        AssetLifecycleService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<AssetLifecycleChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        AssetLifecycleItem? stored = await repository.GetAsync("inventory.assetlifecycle-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<AssetLifecycleItem> repository = new();
        AssetLifecycleService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<AssetLifecycleChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        AssetLifecycleViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}