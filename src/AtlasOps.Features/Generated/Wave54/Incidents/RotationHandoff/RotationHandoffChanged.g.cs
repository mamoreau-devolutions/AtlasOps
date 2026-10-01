namespace AtlasOps.Features.Incidents.RotationHandoff;

public sealed record RotationHandoffChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);