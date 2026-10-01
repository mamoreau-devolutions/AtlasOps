namespace AtlasOps.Features.Platform.WorkspaceLifecycle;

public sealed record WorkspaceLifecycleChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);