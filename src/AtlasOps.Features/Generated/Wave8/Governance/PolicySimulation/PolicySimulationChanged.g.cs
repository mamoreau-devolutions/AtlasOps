namespace AtlasOps.Features.Governance.PolicySimulation;

public sealed record PolicySimulationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);