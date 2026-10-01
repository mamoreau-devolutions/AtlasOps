namespace AtlasOps.Features.Cloud.CloudIdentityOptimization;

public sealed record CloudIdentityOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);