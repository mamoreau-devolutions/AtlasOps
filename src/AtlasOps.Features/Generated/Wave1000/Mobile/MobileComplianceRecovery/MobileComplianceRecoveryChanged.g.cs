namespace AtlasOps.Features.Mobile.MobileComplianceRecovery;

public sealed record MobileComplianceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);