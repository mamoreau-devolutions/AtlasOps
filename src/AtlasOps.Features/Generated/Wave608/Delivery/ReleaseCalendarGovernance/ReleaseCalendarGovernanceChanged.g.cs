namespace AtlasOps.Features.Delivery.ReleaseCalendarGovernance;

public sealed record ReleaseCalendarGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);