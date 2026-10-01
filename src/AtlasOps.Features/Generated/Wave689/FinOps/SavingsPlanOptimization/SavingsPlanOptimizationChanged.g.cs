namespace AtlasOps.Features.FinOps.SavingsPlanOptimization;

public sealed record SavingsPlanOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);