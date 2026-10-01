namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Automation.WorkflowSimulation;

[TestClass]
public sealed class WorkflowSimulationTests
{
    private static UpdateWorkflowSimulationCommand CreateCommand(string targetState = "Ready")
    {
        return new("automation.workflowsimulation-1", "Workflow Simulation", "Operations", targetState, 4, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "automation.workflowsimulation");

        Assert.AreEqual(4, descriptor.Wave);
        Assert.AreEqual("Automation", descriptor.Area);
        Assert.AreEqual(typeof(WorkflowSimulationItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        WorkflowSimulationValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        WorkflowSimulationValidator validator = new();
        UpdateWorkflowSimulationCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        WorkflowSimulationPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkflowSimulationItem> repository = new();
        WorkflowSimulationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkflowSimulationChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        WorkflowSimulationItem? stored = await repository.GetAsync("automation.workflowsimulation-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkflowSimulationItem> repository = new();
        WorkflowSimulationService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkflowSimulationChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        WorkflowSimulationViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}