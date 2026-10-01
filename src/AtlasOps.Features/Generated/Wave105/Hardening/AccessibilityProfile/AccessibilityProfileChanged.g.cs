namespace AtlasOps.Features.Hardening.AccessibilityProfile;

public sealed record AccessibilityProfileChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);