namespace AtlasOps.Features.Hardening.CrashMarker;

public sealed record CrashMarkerChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);