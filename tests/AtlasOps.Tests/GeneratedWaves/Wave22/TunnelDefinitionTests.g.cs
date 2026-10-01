namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Connections.TunnelDefinition;

[TestClass]
public sealed class TunnelDefinitionTests
{
    private static UpdateTunnelDefinitionCommand CreateCommand(string targetState = "Ready")
    {
        return new("connections.tunneldefinition-1", "Tunnel Definition", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "connections.tunneldefinition");

        Assert.AreEqual(22, descriptor.Wave);
        Assert.AreEqual("Connections", descriptor.Area);
        Assert.AreEqual(typeof(TunnelDefinitionItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        TunnelDefinitionValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        TunnelDefinitionValidator validator = new();
        UpdateTunnelDefinitionCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        TunnelDefinitionPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<TunnelDefinitionItem> repository = new();
        TunnelDefinitionService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<TunnelDefinitionChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        TunnelDefinitionItem? stored = await repository.GetAsync("connections.tunneldefinition-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<TunnelDefinitionItem> repository = new();
        TunnelDefinitionService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<TunnelDefinitionChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        TunnelDefinitionViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}