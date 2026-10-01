namespace AtlasOps.Features.Database.DatabaseSchemaRecovery;

public sealed record UpdateDatabaseSchemaRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);