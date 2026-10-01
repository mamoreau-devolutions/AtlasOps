namespace AtlasOps.Features.FinOps.SavingsPlanGovernance;

public sealed record UpdateSavingsPlanGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);