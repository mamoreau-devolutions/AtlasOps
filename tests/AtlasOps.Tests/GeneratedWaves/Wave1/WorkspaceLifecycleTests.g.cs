namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Platform.WorkspaceLifecycle;

[TestClass]
public sealed class WorkspaceLifecycleTests
{
    private static UpdateWorkspaceLifecycleCommand CreateCommand(string targetState = "Ready")
    {
        return new("platform.workspacelifecycle-1", "Workspace Lifecycle", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "platform.workspacelifecycle");

        Assert.AreEqual(1, descriptor.Wave);
        Assert.AreEqual("Platform", descriptor.Area);
        Assert.AreEqual(typeof(WorkspaceLifecycleItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        WorkspaceLifecycleValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        WorkspaceLifecycleValidator validator = new();
        UpdateWorkspaceLifecycleCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        WorkspaceLifecyclePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkspaceLifecycleItem> repository = new();
        WorkspaceLifecycleService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkspaceLifecycleChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        WorkspaceLifecycleItem? stored = await repository.GetAsync("platform.workspacelifecycle-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkspaceLifecycleItem> repository = new();
        WorkspaceLifecycleService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkspaceLifecycleChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        WorkspaceLifecycleViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}