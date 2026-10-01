namespace AtlasOps.Features.FinOps.SavingsPlanRecovery;

public sealed record SavingsPlanRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);