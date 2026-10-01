namespace AtlasOps.Features.Database.DatabaseCredentialOptimization;

public sealed record DatabaseCredentialOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);