namespace AtlasOps.Features.ServiceManagement.ServiceReviewRecovery;

public sealed record UpdateServiceReviewRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);