namespace AtlasOps.Features.FinOps.SavingsPlanProvisioning;

public sealed record SavingsPlanProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);