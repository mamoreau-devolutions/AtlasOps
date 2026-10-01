namespace AtlasOps.Features.Automation.WorkflowVariable;

public sealed record UpdateWorkflowVariableCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);