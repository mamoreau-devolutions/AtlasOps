namespace AtlasOps.Features.Platform.SessionLifecycle;

public sealed record SessionLifecycleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);