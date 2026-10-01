namespace AtlasOps.Features.Connections.RemoteClipboard;

public sealed record RemoteClipboardChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);