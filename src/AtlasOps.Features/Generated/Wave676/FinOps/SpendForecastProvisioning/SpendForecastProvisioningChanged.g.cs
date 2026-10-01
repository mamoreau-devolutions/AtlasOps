namespace AtlasOps.Features.FinOps.SpendForecastProvisioning;

public sealed record SpendForecastProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);