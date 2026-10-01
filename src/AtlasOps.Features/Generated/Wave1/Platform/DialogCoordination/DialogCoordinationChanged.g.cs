namespace AtlasOps.Features.Platform.DialogCoordination;

public sealed record DialogCoordinationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);