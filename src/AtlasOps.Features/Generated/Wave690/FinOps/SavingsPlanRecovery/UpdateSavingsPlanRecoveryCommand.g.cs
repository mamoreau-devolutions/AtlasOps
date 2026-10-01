namespace AtlasOps.Features.FinOps.SavingsPlanRecovery;

public sealed record UpdateSavingsPlanRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);