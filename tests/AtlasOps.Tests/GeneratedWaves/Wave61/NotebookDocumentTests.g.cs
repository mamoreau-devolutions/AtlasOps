namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Editor.NotebookDocument;

[TestClass]
public sealed class NotebookDocumentTests
{
    private static UpdateNotebookDocumentCommand CreateCommand(string targetState = "Ready")
    {
        return new("editor.notebookdocument-1", "Notebook Document", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "editor.notebookdocument");

        Assert.AreEqual(61, descriptor.Wave);
        Assert.AreEqual("Editor", descriptor.Area);
        Assert.AreEqual(typeof(NotebookDocumentItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        NotebookDocumentValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        NotebookDocumentValidator validator = new();
        UpdateNotebookDocumentCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        NotebookDocumentPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<NotebookDocumentItem> repository = new();
        NotebookDocumentService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<NotebookDocumentChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        NotebookDocumentItem? stored = await repository.GetAsync("editor.notebookdocument-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<NotebookDocumentItem> repository = new();
        NotebookDocumentService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<NotebookDocumentChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        NotebookDocumentViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}