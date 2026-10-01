namespace AtlasOps.Features.Delivery.BuildPipelineRecovery;

public sealed record UpdateBuildPipelineRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);