namespace AtlasOps.Features.ServiceManagement.ServiceScorecardRecovery;

public sealed record UpdateServiceScorecardRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);