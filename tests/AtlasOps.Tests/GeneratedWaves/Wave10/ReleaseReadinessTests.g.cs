namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Hardening.ReleaseReadiness;

[TestClass]
public sealed class ReleaseReadinessTests
{
    private static UpdateReleaseReadinessCommand CreateCommand(string targetState = "Ready")
    {
        return new("hardening.releasereadiness-1", "Release Readiness", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "hardening.releasereadiness");

        Assert.AreEqual(10, descriptor.Wave);
        Assert.AreEqual("Hardening", descriptor.Area);
        Assert.AreEqual(typeof(ReleaseReadinessItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ReleaseReadinessValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ReleaseReadinessValidator validator = new();
        UpdateReleaseReadinessCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ReleaseReadinessPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ReleaseReadinessItem> repository = new();
        ReleaseReadinessService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ReleaseReadinessChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ReleaseReadinessItem? stored = await repository.GetAsync("hardening.releasereadiness-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ReleaseReadinessItem> repository = new();
        ReleaseReadinessService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ReleaseReadinessChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ReleaseReadinessViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}