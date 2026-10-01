namespace AtlasOps.Features.Sync.SchemaMigration;

public sealed record UpdateSchemaMigrationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);