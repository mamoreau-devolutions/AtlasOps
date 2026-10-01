namespace AtlasOps.Features.Database.DatabaseIndexRecovery;

public sealed record UpdateDatabaseIndexRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);