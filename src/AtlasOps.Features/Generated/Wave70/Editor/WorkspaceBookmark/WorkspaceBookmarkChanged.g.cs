namespace AtlasOps.Features.Editor.WorkspaceBookmark;

public sealed record WorkspaceBookmarkChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);