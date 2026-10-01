namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Architecture.ArchitectureComponentGovernance;

[TestClass]
public sealed class ArchitectureComponentGovernanceTests
{
    private static UpdateArchitectureComponentGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("architecture.architecturecomponentgovernance-1", "Architecture Component Governance", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "architecture.architecturecomponentgovernance");

        Assert.AreEqual(1068, descriptor.Wave);
        Assert.AreEqual("Architecture", descriptor.Area);
        Assert.AreEqual(typeof(ArchitectureComponentGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ArchitectureComponentGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ArchitectureComponentGovernanceValidator validator = new();
        UpdateArchitectureComponentGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ArchitectureComponentGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ArchitectureComponentGovernanceItem> repository = new();
        ArchitectureComponentGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ArchitectureComponentGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ArchitectureComponentGovernanceItem? stored = await repository.GetAsync("architecture.architecturecomponentgovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ArchitectureComponentGovernanceItem> repository = new();
        ArchitectureComponentGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ArchitectureComponentGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ArchitectureComponentGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}