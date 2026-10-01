namespace AtlasOps.Features.FinOps.SpendForecastRecovery;

public sealed record UpdateSpendForecastRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);