namespace AtlasOps.Features.Database.DatabaseSchemaOptimization;

public sealed record UpdateDatabaseSchemaOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);