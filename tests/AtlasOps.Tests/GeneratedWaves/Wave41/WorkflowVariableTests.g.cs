namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Automation.WorkflowVariable;

[TestClass]
public sealed class WorkflowVariableTests
{
    private static UpdateWorkflowVariableCommand CreateCommand(string targetState = "Ready")
    {
        return new("automation.workflowvariable-1", "Workflow Variable", "Operations", targetState, 1, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "automation.workflowvariable");

        Assert.AreEqual(41, descriptor.Wave);
        Assert.AreEqual("Automation", descriptor.Area);
        Assert.AreEqual(typeof(WorkflowVariableItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        WorkflowVariableValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        WorkflowVariableValidator validator = new();
        UpdateWorkflowVariableCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        WorkflowVariablePolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkflowVariableItem> repository = new();
        WorkflowVariableService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkflowVariableChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        WorkflowVariableItem? stored = await repository.GetAsync("automation.workflowvariable-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<WorkflowVariableItem> repository = new();
        WorkflowVariableService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<WorkflowVariableChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        WorkflowVariableViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}