namespace AtlasOps.Features.Sync.SchemaMigration;

public sealed record SchemaMigrationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);