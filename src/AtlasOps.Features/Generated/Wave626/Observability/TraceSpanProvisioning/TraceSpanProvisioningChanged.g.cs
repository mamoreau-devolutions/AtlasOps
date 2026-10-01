namespace AtlasOps.Features.Observability.TraceSpanProvisioning;

public sealed record TraceSpanProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);