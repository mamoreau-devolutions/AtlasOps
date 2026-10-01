namespace AtlasOps.Features.Security.SecurityIdentityOptimization;

public sealed record SecurityIdentityOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);