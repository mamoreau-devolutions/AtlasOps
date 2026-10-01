namespace AtlasOps.Features.Delivery.ReleasePipelineRecovery;

public sealed record UpdateReleasePipelineRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);