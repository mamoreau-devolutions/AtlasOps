namespace AtlasOps.Features.Database.DatabaseRestoreRecovery;

public sealed record UpdateDatabaseRestoreRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);