namespace AtlasOps.Features.Hardening.KeyboardMap;

public sealed record KeyboardMapChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);