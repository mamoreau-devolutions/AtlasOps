namespace AtlasOps.Features.Platform.BackgroundOperation;

public sealed record BackgroundOperationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);