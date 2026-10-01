namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Data.DataProductOptimization;

[TestClass]
public sealed class DataProductOptimizationTests
{
    private static UpdateDataProductOptimizationCommand CreateCommand(string targetState = "Ready")
    {
        return new("data.dataproductoptimization-1", "Data Product Optimization", "Operations", targetState, 9, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "data.dataproductoptimization");

        Assert.AreEqual(809, descriptor.Wave);
        Assert.AreEqual("Data", descriptor.Area);
        Assert.AreEqual(typeof(DataProductOptimizationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        DataProductOptimizationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        DataProductOptimizationValidator validator = new();
        UpdateDataProductOptimizationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        DataProductOptimizationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DataProductOptimizationItem> repository = new();
        DataProductOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DataProductOptimizationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        DataProductOptimizationItem? stored = await repository.GetAsync("data.dataproductoptimization-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DataProductOptimizationItem> repository = new();
        DataProductOptimizationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DataProductOptimizationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        DataProductOptimizationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}