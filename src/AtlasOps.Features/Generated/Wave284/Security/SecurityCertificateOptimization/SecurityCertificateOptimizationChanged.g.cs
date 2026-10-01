namespace AtlasOps.Features.Security.SecurityCertificateOptimization;

public sealed record SecurityCertificateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);