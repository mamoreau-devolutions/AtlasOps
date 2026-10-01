namespace AtlasOps.Features.Observability.ObservabilityRetentionProvisioning;

public sealed record ObservabilityRetentionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);