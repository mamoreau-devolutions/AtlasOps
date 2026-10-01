namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Hardening.UpgradeChannel;

[TestClass]
public sealed class UpgradeChannelTests
{
    private static UpdateUpgradeChannelCommand CreateCommand(string targetState = "Ready")
    {
        return new("hardening.upgradechannel-1", "Upgrade Channel", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "hardening.upgradechannel");

        Assert.AreEqual(108, descriptor.Wave);
        Assert.AreEqual("Hardening", descriptor.Area);
        Assert.AreEqual(typeof(UpgradeChannelItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        UpgradeChannelValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        UpgradeChannelValidator validator = new();
        UpdateUpgradeChannelCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        UpgradeChannelPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<UpgradeChannelItem> repository = new();
        UpgradeChannelService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<UpgradeChannelChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        UpgradeChannelItem? stored = await repository.GetAsync("hardening.upgradechannel-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<UpgradeChannelItem> repository = new();
        UpgradeChannelService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<UpgradeChannelChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        UpgradeChannelViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}