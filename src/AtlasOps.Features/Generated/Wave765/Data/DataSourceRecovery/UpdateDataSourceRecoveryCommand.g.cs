namespace AtlasOps.Features.Data.DataSourceRecovery;

public sealed record UpdateDataSourceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);