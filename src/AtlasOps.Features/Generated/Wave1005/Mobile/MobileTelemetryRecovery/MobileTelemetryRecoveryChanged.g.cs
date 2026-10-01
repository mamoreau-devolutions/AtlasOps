namespace AtlasOps.Features.Mobile.MobileTelemetryRecovery;

public sealed record MobileTelemetryRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);