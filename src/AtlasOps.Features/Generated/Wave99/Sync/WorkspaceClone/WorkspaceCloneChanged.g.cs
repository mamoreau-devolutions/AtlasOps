namespace AtlasOps.Features.Sync.WorkspaceClone;

public sealed record WorkspaceCloneChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);