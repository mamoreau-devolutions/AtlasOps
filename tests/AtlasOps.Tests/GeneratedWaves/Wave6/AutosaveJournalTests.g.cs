namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Editor.AutosaveJournal;

[TestClass]
public sealed class AutosaveJournalTests
{
    private static UpdateAutosaveJournalCommand CreateCommand(string targetState = "Ready")
    {
        return new("editor.autosavejournal-1", "Autosave Journal", "Operations", targetState, 6, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "editor.autosavejournal");

        Assert.AreEqual(6, descriptor.Wave);
        Assert.AreEqual("Editor", descriptor.Area);
        Assert.AreEqual(typeof(AutosaveJournalItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        AutosaveJournalValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        AutosaveJournalValidator validator = new();
        UpdateAutosaveJournalCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        AutosaveJournalPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<AutosaveJournalItem> repository = new();
        AutosaveJournalService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<AutosaveJournalChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        AutosaveJournalItem? stored = await repository.GetAsync("editor.autosavejournal-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<AutosaveJournalItem> repository = new();
        AutosaveJournalService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<AutosaveJournalChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        AutosaveJournalViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}