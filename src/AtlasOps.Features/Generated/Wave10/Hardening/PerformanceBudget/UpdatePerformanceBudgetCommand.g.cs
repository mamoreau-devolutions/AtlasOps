namespace AtlasOps.Features.Hardening.PerformanceBudget;

public sealed record UpdatePerformanceBudgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);