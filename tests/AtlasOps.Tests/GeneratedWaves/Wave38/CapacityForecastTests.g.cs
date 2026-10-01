namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Inventory.CapacityForecast;

[TestClass]
public sealed class CapacityForecastTests
{
    private static UpdateCapacityForecastCommand CreateCommand(string targetState = "Ready")
    {
        return new("inventory.capacityforecast-1", "Capacity Forecast", "Operations", targetState, 8, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "inventory.capacityforecast");

        Assert.AreEqual(38, descriptor.Wave);
        Assert.AreEqual("Inventory", descriptor.Area);
        Assert.AreEqual(typeof(CapacityForecastItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        CapacityForecastValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        CapacityForecastValidator validator = new();
        UpdateCapacityForecastCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        CapacityForecastPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CapacityForecastItem> repository = new();
        CapacityForecastService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CapacityForecastChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        CapacityForecastItem? stored = await repository.GetAsync("inventory.capacityforecast-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<CapacityForecastItem> repository = new();
        CapacityForecastService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<CapacityForecastChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        CapacityForecastViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}