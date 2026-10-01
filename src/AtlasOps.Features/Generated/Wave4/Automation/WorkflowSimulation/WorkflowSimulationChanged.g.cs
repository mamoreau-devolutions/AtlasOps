namespace AtlasOps.Features.Automation.WorkflowSimulation;

public sealed record WorkflowSimulationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);