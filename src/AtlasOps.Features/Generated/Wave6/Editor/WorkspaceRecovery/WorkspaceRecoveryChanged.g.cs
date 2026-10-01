namespace AtlasOps.Features.Editor.WorkspaceRecovery;

public sealed record WorkspaceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);