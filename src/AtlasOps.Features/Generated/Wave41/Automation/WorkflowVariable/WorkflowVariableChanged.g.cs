namespace AtlasOps.Features.Automation.WorkflowVariable;

public sealed record WorkflowVariableChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);