namespace AtlasOps.Features.Automation.WorkflowSimulation;

public sealed record UpdateWorkflowSimulationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);