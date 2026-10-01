namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Inventory.DependencyMapping;

[TestClass]
public sealed class DependencyMappingTests
{
    private static UpdateDependencyMappingCommand CreateCommand(string targetState = "Ready")
    {
        return new("inventory.dependencymapping-1", "Dependency Mapping", "Operations", targetState, 3, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "inventory.dependencymapping");

        Assert.AreEqual(3, descriptor.Wave);
        Assert.AreEqual("Inventory", descriptor.Area);
        Assert.AreEqual(typeof(DependencyMappingItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        DependencyMappingValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        DependencyMappingValidator validator = new();
        UpdateDependencyMappingCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        DependencyMappingPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DependencyMappingItem> repository = new();
        DependencyMappingService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DependencyMappingChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        DependencyMappingItem? stored = await repository.GetAsync("inventory.dependencymapping-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<DependencyMappingItem> repository = new();
        DependencyMappingService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<DependencyMappingChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        DependencyMappingViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}