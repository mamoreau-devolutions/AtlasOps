namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Compute.ComputeScaleSetRecovery;

[TestClass]
public sealed class ComputeScaleSetRecoveryTests
{
    private static UpdateComputeScaleSetRecoveryCommand CreateCommand(string targetState = "Ready")
    {
        return new("compute.computescalesetrecovery-1", "Compute Scale Set Recovery", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "compute.computescalesetrecovery");

        Assert.AreEqual(530, descriptor.Wave);
        Assert.AreEqual("Compute", descriptor.Area);
        Assert.AreEqual(typeof(ComputeScaleSetRecoveryItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ComputeScaleSetRecoveryValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ComputeScaleSetRecoveryValidator validator = new();
        UpdateComputeScaleSetRecoveryCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ComputeScaleSetRecoveryPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ComputeScaleSetRecoveryItem> repository = new();
        ComputeScaleSetRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ComputeScaleSetRecoveryChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ComputeScaleSetRecoveryItem? stored = await repository.GetAsync("compute.computescalesetrecovery-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ComputeScaleSetRecoveryItem> repository = new();
        ComputeScaleSetRecoveryService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ComputeScaleSetRecoveryChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ComputeScaleSetRecoveryViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}