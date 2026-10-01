namespace AtlasOps.Features.Mobile.MobileComplianceOptimization;

public sealed record MobileComplianceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);