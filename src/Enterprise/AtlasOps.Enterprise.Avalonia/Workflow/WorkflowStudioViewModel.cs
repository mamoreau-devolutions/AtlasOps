namespace AtlasOps.Enterprise.Avalonia.Workflow;

using AtlasOps.Enterprise.Avalonia.Common;
using AtlasOps.Enterprise.Contracts.Workflow;
using AtlasOps.Enterprise.Core.Scenarios;
using AtlasOps.Enterprise.Core.Workflow;

using global::Avalonia.Collections;

public sealed class WorkflowStudioViewModel : EnterpriseViewModelBase
{
    private readonly WorkflowGraphValidator validator = new();
    private readonly WorkflowPlanner planner = new();
    private readonly WorkflowFailurePlanner failurePlanner = new();
    private readonly IReadOnlyList<WorkflowDefinition> allWorkflows;
    private WorkflowDefinition? selectedWorkflow;

    public WorkflowStudioViewModel()
    {
        this.allWorkflows =
        [
            EnterpriseScenarioCatalog.CreateDeploymentWorkflow(),
            EnterpriseScenarioCatalog.CreateInvalidWorkflow(),
        ];
        this.RefreshWorkflows();
        this.SelectedWorkflow = this.Workflows.FirstOrDefault();
    }

    public override string Title => "Workflow orchestration studio";

    public override string Summary => "Validate directed execution graphs, inspect parallel layers, and simulate retry and compensation behavior.";

    public AvaloniaList<WorkflowDefinition> Workflows { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Steps { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> ExecutionLayers { get; } = [];

    public AvaloniaList<EnterpriseDetailRow> Diagnostics { get; } = [];

    public AvaloniaList<EnterpriseMetric> Metrics { get; } = [];

    public WorkflowDefinition? SelectedWorkflow
    {
        get => this.selectedWorkflow;
        set
        {
            if (this.selectedWorkflow == value)
            {
                return;
            }

            this.selectedWorkflow = value;
            this.OnPropertyChanged();
            this.RefreshSelection();
        }
    }

    public string FailurePlan { get; private set; } = "Select a workflow.";

    protected override void OnSearchChanged()
    {
        this.RefreshWorkflows();
    }

    private void RefreshWorkflows()
    {
        IEnumerable<WorkflowDefinition> filtered = this.allWorkflows;
        if (!string.IsNullOrWhiteSpace(this.SearchText))
        {
            filtered = filtered.Where(
                workflow =>
                    workflow.DisplayName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase) ||
                    workflow.Steps.Any(step => step.DisplayName.Contains(this.SearchText, StringComparison.OrdinalIgnoreCase)));
        }

        this.Workflows.Clear();
        this.Workflows.AddRange(filtered);
        if (this.SelectedWorkflow is not null && !this.Workflows.Contains(this.SelectedWorkflow))
        {
            this.SelectedWorkflow = this.Workflows.FirstOrDefault();
        }
    }

    private void RefreshSelection()
    {
        this.Steps.Clear();
        this.ExecutionLayers.Clear();
        this.Diagnostics.Clear();
        this.Metrics.Clear();

        if (this.SelectedWorkflow is null)
        {
            return;
        }

        WorkflowValidationResult validation = this.validator.Validate(this.SelectedWorkflow);
        WorkflowExecutionPlan plan = this.planner.CreatePlan(this.SelectedWorkflow);
        this.Steps.AddRange(
            this.SelectedWorkflow.Steps.Select(
                step => new EnterpriseDetailRow(step.DisplayName, $"{step.Kind} · {step.Handler}", step.Id)));
        this.ExecutionLayers.AddRange(
            plan.Layers.Select(
                layer => new EnterpriseDetailRow(
                    $"Layer {layer.Index + 1}",
                    string.Join(" → ", layer.Steps.Select(static step => step.DisplayName)),
                    $"{layer.Steps.Count} parallel step(s)")));
        this.Diagnostics.AddRange(
            validation.Diagnostics.Select(
                diagnostic => new EnterpriseDetailRow(diagnostic.Code, diagnostic.Message, diagnostic.Severity.ToString())));
        this.Metrics.AddRange(
        [
            new("Steps", this.SelectedWorkflow.Steps.Count.ToString(), "Executable and control nodes"),
            new("Edges", this.SelectedWorkflow.Edges.Count.ToString(), "Declared graph transitions"),
            new("Layers", plan.Layers.Count.ToString(), "Parallel execution groups"),
            new("Validation", validation.IsValid ? "PASS" : "FAIL", $"{validation.Diagnostics.Count} diagnostic(s)"),
        ]);

        WorkflowStep? failureStep = this.SelectedWorkflow.Steps.FirstOrDefault(static step => step.Id == "deploy")
            ?? this.SelectedWorkflow.Steps.FirstOrDefault();
        if (failureStep is not null)
        {
            WorkflowFailurePlan failure = this.failurePlanner.Plan(this.SelectedWorkflow, failureStep.Id, 1, DateTimeOffset.UtcNow);
            this.FailurePlan = $"{failure.Action}: {failure.Explanation}";
            this.OnPropertyChanged(nameof(this.FailurePlan));
        }
    }
}
