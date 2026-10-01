namespace AtlasOps.Features.Editor.SessionTranscript;

public sealed record UpdateSessionTranscriptCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);