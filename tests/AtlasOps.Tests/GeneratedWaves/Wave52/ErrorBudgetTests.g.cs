namespace AtlasOps.Tests.GeneratedWaves;

using AtlasOps.Features;
using AtlasOps.Features.Incidents.ErrorBudget;

[TestClass]
public sealed class ErrorBudgetTests
{
    private static UpdateErrorBudgetCommand CreateCommand(string targetState = "Ready")
    {
        return new("incidents.errorbudget-1", "Error Budget", "Operations", targetState, 2, true);
    }

    [TestMethod]
    public void Descriptor_HasExpectedWaveAndArea()
    {
        AtlasOpsCapabilityDescriptor descriptor = AtlasOpsCapabilityCatalog.All.Single(static item => item.Id == "incidents.errorbudget");

        Assert.AreEqual(52, descriptor.Wave);
        Assert.AreEqual("Incidents", descriptor.Area);
        Assert.AreEqual(typeof(ErrorBudgetItem), descriptor.ModelType);
    }

    [TestMethod]
    public void Validator_AcceptsValidCommand()
    {
        ErrorBudgetValidator validator = new();

        Assert.AreEqual(0, validator.Validate(CreateCommand()).Count);
    }

    [DataTestMethod]
    [DataRow("", "Operations", "Ready", 1, "Name")]
    [DataRow("Name", "", "Ready", 1, "Owner")]
    [DataRow("Name", "Operations", "", 1, "State")]
    [DataRow("Name", "Operations", "Ready", 0, "Priority")]
    public void Validator_RejectsInvalidFields(string name, string owner, string state, int priority, string expectedField)
    {
        ErrorBudgetValidator validator = new();
        UpdateErrorBudgetCommand command = new("id", name, owner, state, priority, true);

        Assert.IsTrue(validator.Validate(command).Any(issue => issue.Field == expectedField));
    }

    [TestMethod]
    public void Policy_ExposesDeterministicTransitions()
    {
        ErrorBudgetPolicy policy = new();

        CollectionAssert.AreEqual(new[] { "Cancelled", "Ready" }, policy.GetAvailableTransitions("Draft").ToArray());
    }

    [TestMethod]
    public async Task Service_PersistsValidTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ErrorBudgetItem> repository = new();
        ErrorBudgetService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ErrorBudgetChanged> result = await service.ExecuteAsync(CreateCommand(), "operator", CancellationToken.None);
        ErrorBudgetItem? stored = await repository.GetAsync("incidents.errorbudget-1", CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(stored);
        Assert.AreEqual("Ready", stored.State);
        Assert.AreEqual(1, stored.Revision);
    }

    [TestMethod]
    public async Task Service_RejectsIllegalTransition()
    {
        InMemoryAtlasOpsCapabilityRepository<ErrorBudgetItem> repository = new();
        ErrorBudgetService service = new(repository, TimeProvider.System);

        AtlasOpsOperationResult<ErrorBudgetChanged> result = await service.ExecuteAsync(CreateCommand("Completed"), "operator", CancellationToken.None);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Issues.Any(issue => issue.Field == "State"));
    }

    [TestMethod]
    public void ViewModel_AdvancesUsingPolicy()
    {
        ErrorBudgetViewModel viewModel = new();

        viewModel.Advance();

        Assert.AreEqual("Cancelled", viewModel.State);
        StringAssert.Contains(viewModel.Status, "Cancelled");
    }
}