namespace AtlasOps.Features.Editor.SessionTranscript;

public sealed record SessionTranscriptChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);