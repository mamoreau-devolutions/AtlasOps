namespace AtlasOps.Features.Edge.EdgeTelemetryProvisioning;

public sealed record EdgeTelemetryProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);