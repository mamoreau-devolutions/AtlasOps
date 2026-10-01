namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Hardening.KeyboardMap;

[TestClass]
public sealed class KeyboardMapTests
{
    private static UpdateKeyboardMapCommand CreateCommand(string targetState = "Ready")
    {
        return new("hardening.keyboardmap-1", "Keyboard Map", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "hardening.keyboardmap");

        Assert.AreEqual(106, descriptor.Wave);
        Assert.AreEqual("Hardening", descriptor.Area);
        Assert.AreEqual(typeof(KeyboardMapItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        KeyboardMapValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        KeyboardMapValidator validator = new();
        UpdateKeyboardMapCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        KeyboardMapPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KeyboardMapItem> repository = new();
        KeyboardMapService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KeyboardMapChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        KeyboardMapItem? stored = await repository.GetAsync("hardening.keyboardmap-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<KeyboardMapItem> repository = new();
        KeyboardMapService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<KeyboardMapChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        KeyboardMapViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}