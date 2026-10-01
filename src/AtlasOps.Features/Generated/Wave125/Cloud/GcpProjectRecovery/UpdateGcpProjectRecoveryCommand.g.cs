namespace AtlasOps.Features.Cloud.GcpProjectRecovery;

public sealed record UpdateGcpProjectRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);