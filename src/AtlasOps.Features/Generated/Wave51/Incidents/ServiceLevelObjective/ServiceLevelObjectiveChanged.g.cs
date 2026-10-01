namespace AtlasOps.Features.Incidents.ServiceLevelObjective;

public sealed record ServiceLevelObjectiveChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);