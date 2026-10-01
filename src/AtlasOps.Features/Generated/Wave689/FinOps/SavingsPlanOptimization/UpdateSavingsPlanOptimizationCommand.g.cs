namespace AtlasOps.Features.FinOps.SavingsPlanOptimization;

public sealed record UpdateSavingsPlanOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);