namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Storage.FileShareOptimization;

[TestClass]
public sealed class FileShareOptimizationTests
{
    private static UpdateFileShareOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("storage.fileshareoptimization-1", "File Share Optimization", "Operations", targetState, 9, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "storage.fileshareoptimization");

        Assert.AreEqual(369, descriptor.Wave);
        Assert.AreEqual("Storage", descriptor.Area);
        Assert.AreEqual(typeof(FileShareOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        FileShareOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        FileShareOptimizationValidator validator = new();
        UpdateFileShareOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        FileShareOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<FileShareOptimizationItem> repository = new();
        FileShareOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<FileShareOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        FileShareOptimizationItem? stored = await repository.GetAsync("storage.fileshareoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<FileShareOptimizationItem> repository = new();
        FileShareOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<FileShareOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        FileShareOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}