namespace AtlasOps.Features.FinOps.SpendForecastGovernance;

public sealed record UpdateSpendForecastGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);