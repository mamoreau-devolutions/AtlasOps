namespace AtlasOps.Features.Delivery.SourceRepositoryRecovery;

public sealed record UpdateSourceRepositoryRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);