namespace AtlasOps.Features.Database.DatabaseQueryRecovery;

public sealed record UpdateDatabaseQueryRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);