namespace AtlasOps.Features.Sync.DisasterRecovery;

public sealed record UpdateDisasterRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);