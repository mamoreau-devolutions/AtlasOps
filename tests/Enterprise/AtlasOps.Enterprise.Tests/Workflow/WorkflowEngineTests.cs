namespace AtlasOps.Enterprise.Tests.Workflow;

using AtlasOps.Enterprise.Contracts.Workflow;
using AtlasOps.Enterprise.Core.Scenarios;
using AtlasOps.Enterprise.Core.Workflow;

[TestClass]
public sealed class WorkflowEngineTests
{
    [TestMethod]
    public void Validate_DeploymentWorkflow_ReturnsValidGraph()
    {
        WorkflowDefinition workflow = EnterpriseScenarioCatalog.CreateDeploymentWorkflow();
        WorkflowGraphValidator validator = new();

        WorkflowValidationResult result = validator.Validate(workflow);

        Assert.IsTrue(result.IsValid);
        Assert.Contains("validate", result.EntryStepIds);
        Assert.Contains("notify", result.TerminalStepIds);
        Assert.Contains("rollback", result.TerminalStepIds);
    }

    [TestMethod]
    public void Validate_CyclicWorkflow_ReturnsCycleDiagnostic()
    {
        WorkflowDefinition workflow = EnterpriseScenarioCatalog.CreateInvalidWorkflow();
        WorkflowGraphValidator validator = new();

        WorkflowValidationResult result = validator.Validate(workflow);

        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Diagnostics.Any(static diagnostic => diagnostic.Code == "workflow.cycle"));
    }

    [TestMethod]
    public void CreatePlan_DeploymentWorkflow_CreatesStableExecutionLayers()
    {
        WorkflowDefinition workflow = EnterpriseScenarioCatalog.CreateDeploymentWorkflow();
        WorkflowPlanner planner = new();

        WorkflowExecutionPlan result = planner.CreatePlan(workflow);

        Assert.HasCount(5, result.Layers);
        Assert.AreEqual("validate", result.Layers[0].Steps[0].Id);
        Assert.AreEqual("notify", result.Layers[^1].Steps[0].Id);
        Assert.HasCount(1, result.Dependencies["deploy"]);
        Assert.AreEqual("approve", result.Dependencies["deploy"][0]);
    }

    [TestMethod]
    public void Evaluate_NumericCondition_UsesTypedComparison()
    {
        WorkflowConditionEvaluator evaluator = new();
        WorkflowConditionContext context = new(
            new Dictionary<string, object?>
            {
                ["healthScore"] = 97.5m,
                ["approved"] = true,
            });

        WorkflowConditionResult numeric = evaluator.Evaluate("healthScore >= 95", context);
        WorkflowConditionResult boolean = evaluator.Evaluate("approved == true", context);

        Assert.IsTrue(numeric.IsValid);
        Assert.IsTrue(numeric.Value);
        Assert.IsTrue(boolean.Value);
    }

    [TestMethod]
    public void Plan_RetriesExhausted_ReturnsCompensation()
    {
        WorkflowDefinition workflow = EnterpriseScenarioCatalog.CreateDeploymentWorkflow();
        WorkflowFailurePlanner planner = new();

        WorkflowFailurePlan result = planner.Plan(workflow, "deploy", 3, DateTimeOffset.Parse("2026-10-01T12:00:00Z"));

        Assert.AreEqual(WorkflowFailureAction.Compensate, result.Action);
        Assert.Contains("rollback", result.CompensationStepIds);
        Assert.IsEmpty(result.Retries);
    }
}
