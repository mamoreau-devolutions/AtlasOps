namespace AtlasOps.Features.Mobile.MobileCertificateOptimization;

public sealed record MobileCertificateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);