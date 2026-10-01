namespace AtlasOps.Features.Observability.ObservabilityExportProvisioning;

public sealed record ObservabilityExportProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);