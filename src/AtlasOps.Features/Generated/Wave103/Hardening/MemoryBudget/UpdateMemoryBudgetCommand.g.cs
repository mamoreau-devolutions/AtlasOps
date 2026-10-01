namespace AtlasOps.Features.Hardening.MemoryBudget;

public sealed record UpdateMemoryBudgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);