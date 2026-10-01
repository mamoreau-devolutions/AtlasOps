namespace AtlasOps.Features.Observability.MetricSourceProvisioning;

public sealed record MetricSourceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);