namespace AtlasOps.Features.Observability.ObservabilitySloProvisioning;

public sealed record ObservabilitySloProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);