namespace AtlasOps.Features.Incidents.ErrorBudget;

public sealed record UpdateErrorBudgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);