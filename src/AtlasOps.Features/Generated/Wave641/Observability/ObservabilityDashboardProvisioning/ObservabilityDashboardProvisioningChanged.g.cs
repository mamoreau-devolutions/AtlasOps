namespace AtlasOps.Features.Observability.ObservabilityDashboardProvisioning;

public sealed record ObservabilityDashboardProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);