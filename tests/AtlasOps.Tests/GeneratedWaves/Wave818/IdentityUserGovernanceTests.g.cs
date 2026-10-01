namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Identity.IdentityUserGovernance;

[TestClass]
public sealed class IdentityUserGovernanceTests
{
    private static UpdateIdentityUserGovernanceCommand CreateCommand(string targetState = "Ready")
    {
        return new("identity.identityusergovernance-1", "Identity User Governance", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "identity.identityusergovernance");

        Assert.AreEqual(818, descriptor.Wave);
        Assert.AreEqual("Identity", descriptor.Area);
        Assert.AreEqual(typeof(IdentityUserGovernanceItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        IdentityUserGovernanceValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        IdentityUserGovernanceValidator validator = new();
        UpdateIdentityUserGovernanceCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        IdentityUserGovernancePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<IdentityUserGovernanceItem> repository = new();
        IdentityUserGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<IdentityUserGovernanceChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        IdentityUserGovernanceItem? stored = await repository.GetAsync("identity.identityusergovernance-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<IdentityUserGovernanceItem> repository = new();
        IdentityUserGovernanceService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<IdentityUserGovernanceChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        IdentityUserGovernanceViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}