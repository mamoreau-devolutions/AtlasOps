namespace AtlasOps.Features.BusinessContinuity.RecoverySiteRecovery;

public sealed record UpdateRecoverySiteRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);