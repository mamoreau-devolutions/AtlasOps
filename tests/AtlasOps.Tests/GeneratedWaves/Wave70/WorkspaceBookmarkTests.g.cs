namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Editor.WorkspaceBookmark;

[TestClass]
public sealed class WorkspaceBookmarkTests
{
    private static UpdateWorkspaceBookmarkCommand CreateCommand(string targetState = "Ready")
    {
        return new("editor.workspacebookmark-1", "Workspace Bookmark", "Operations", targetState, 10, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "editor.workspacebookmark");

        Assert.AreEqual(70, descriptor.Wave);
        Assert.AreEqual("Editor", descriptor.Area);
        Assert.AreEqual(typeof(WorkspaceBookmarkItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        WorkspaceBookmarkValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        WorkspaceBookmarkValidator validator = new();
        UpdateWorkspaceBookmarkCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        WorkspaceBookmarkPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkspaceBookmarkItem> repository = new();
        WorkspaceBookmarkService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkspaceBookmarkChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        WorkspaceBookmarkItem? stored = await repository.GetAsync("editor.workspacebookmark-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkspaceBookmarkItem> repository = new();
        WorkspaceBookmarkService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkspaceBookmarkChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        WorkspaceBookmarkViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}