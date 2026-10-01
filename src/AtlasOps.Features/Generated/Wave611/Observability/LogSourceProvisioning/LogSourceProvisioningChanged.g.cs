namespace AtlasOps.Features.Observability.LogSourceProvisioning;

public sealed record LogSourceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);