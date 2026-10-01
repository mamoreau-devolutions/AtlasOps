namespace AtlasOps.Features.FinOps.SavingsPlanGovernance;

public sealed record SavingsPlanGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);