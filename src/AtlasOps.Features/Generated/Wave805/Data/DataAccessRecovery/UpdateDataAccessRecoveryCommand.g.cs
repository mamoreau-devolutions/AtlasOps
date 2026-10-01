namespace AtlasOps.Features.Data.DataAccessRecovery;

public sealed record UpdateDataAccessRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);