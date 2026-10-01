namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Platform.OperationThrottling;

[TestClass]
public sealed class OperationThrottlingTests
{
    private static UpdateOperationThrottlingCommand CreateCommand(string targetState = "Ready")
    {
        return new("platform.operationthrottling-1", "Operation Throttling", "Operations", targetState, 5, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "platform.operationthrottling");

        Assert.AreEqual(15, descriptor.Wave);
        Assert.AreEqual("Platform", descriptor.Area);
        Assert.AreEqual(typeof(OperationThrottlingItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        OperationThrottlingValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        OperationThrottlingValidator validator = new();
        UpdateOperationThrottlingCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        OperationThrottlingPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<OperationThrottlingItem> repository = new();
        OperationThrottlingService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<OperationThrottlingChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        OperationThrottlingItem? stored = await repository.GetAsync("platform.operationthrottling-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<OperationThrottlingItem> repository = new();
        OperationThrottlingService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<OperationThrottlingChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        OperationThrottlingViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}