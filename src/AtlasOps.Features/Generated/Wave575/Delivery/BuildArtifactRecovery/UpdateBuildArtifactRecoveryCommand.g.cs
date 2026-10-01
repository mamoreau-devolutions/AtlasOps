namespace AtlasOps.Features.Delivery.BuildArtifactRecovery;

public sealed record UpdateBuildArtifactRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);