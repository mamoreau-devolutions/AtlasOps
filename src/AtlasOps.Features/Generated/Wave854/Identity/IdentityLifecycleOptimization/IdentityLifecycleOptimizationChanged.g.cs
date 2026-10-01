namespace AtlasOps.Features.Identity.IdentityLifecycleOptimization;

public sealed record IdentityLifecycleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);