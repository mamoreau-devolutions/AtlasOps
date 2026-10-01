namespace AtlasOps.Features.Database.DatabaseCredentialOptimization;

public sealed record UpdateDatabaseCredentialOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);