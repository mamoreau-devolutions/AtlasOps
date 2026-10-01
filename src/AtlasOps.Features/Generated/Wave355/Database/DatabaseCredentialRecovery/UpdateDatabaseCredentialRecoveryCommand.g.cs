namespace AtlasOps.Features.Database.DatabaseCredentialRecovery;

public sealed record UpdateDatabaseCredentialRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);