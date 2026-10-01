namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Governance.SegregationOfDuties;

[TestClass]
public sealed class SegregationOfDutiesTests
{
    private static UpdateSegregationOfDutiesCommand CreateCommand(string targetState = "Ready")
    {
        return new("governance.segregationofduties-1", "Segregation Of Duties", "Operations", targetState, 7, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "governance.segregationofduties");

        Assert.AreEqual(87, descriptor.Wave);
        Assert.AreEqual("Governance", descriptor.Area);
        Assert.AreEqual(typeof(SegregationOfDutiesItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        SegregationOfDutiesValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        SegregationOfDutiesValidator validator = new();
        UpdateSegregationOfDutiesCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        SegregationOfDutiesPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SegregationOfDutiesItem> repository = new();
        SegregationOfDutiesService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SegregationOfDutiesChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        SegregationOfDutiesItem? stored = await repository.GetAsync("governance.segregationofduties-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<SegregationOfDutiesItem> repository = new();
        SegregationOfDutiesService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<SegregationOfDutiesChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        SegregationOfDutiesViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}