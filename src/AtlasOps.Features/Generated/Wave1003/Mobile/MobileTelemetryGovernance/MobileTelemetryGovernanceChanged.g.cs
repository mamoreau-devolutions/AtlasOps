namespace AtlasOps.Features.Mobile.MobileTelemetryGovernance;

public sealed record MobileTelemetryGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);