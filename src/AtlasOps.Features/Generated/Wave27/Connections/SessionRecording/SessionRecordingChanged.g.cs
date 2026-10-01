namespace AtlasOps.Features.Connections.SessionRecording;

public sealed record SessionRecordingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);