namespace AtlasOps.Features.Security.SecurityPatchOptimization;

public sealed record SecurityPatchOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);