namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Governance.GovernanceAttestation;

[TestClass]
public sealed class GovernanceAttestationTests
{
    private static UpdateGovernanceAttestationCommand CreateCommand(string targetState = "Ready")
    {
        return new("governance.governanceattestation-1", "Governance Attestation", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "governance.governanceattestation");

        Assert.AreEqual(90, descriptor.Wave);
        Assert.AreEqual("Governance", descriptor.Area);
        Assert.AreEqual(typeof(GovernanceAttestationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        GovernanceAttestationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        GovernanceAttestationValidator validator = new();
        UpdateGovernanceAttestationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        GovernanceAttestationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<GovernanceAttestationItem> repository = new();
        GovernanceAttestationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<GovernanceAttestationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        GovernanceAttestationItem? stored = await repository.GetAsync("governance.governanceattestation-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<GovernanceAttestationItem> repository = new();
        GovernanceAttestationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<GovernanceAttestationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        GovernanceAttestationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}