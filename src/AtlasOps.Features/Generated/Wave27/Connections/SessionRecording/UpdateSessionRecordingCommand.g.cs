namespace AtlasOps.Features.Connections.SessionRecording;

public sealed record UpdateSessionRecordingCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);