namespace AtlasOps.Features.Observability.TraceSourceProvisioning;

public sealed record TraceSourceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);