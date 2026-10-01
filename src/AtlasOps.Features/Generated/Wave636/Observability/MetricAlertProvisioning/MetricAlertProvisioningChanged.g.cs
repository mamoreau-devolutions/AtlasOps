namespace AtlasOps.Features.Observability.MetricAlertProvisioning;

public sealed record MetricAlertProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);