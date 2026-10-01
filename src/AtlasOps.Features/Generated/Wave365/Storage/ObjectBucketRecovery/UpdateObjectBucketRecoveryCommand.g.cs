namespace AtlasOps.Features.Storage.ObjectBucketRecovery;

public sealed record UpdateObjectBucketRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);