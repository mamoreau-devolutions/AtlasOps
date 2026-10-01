namespace AtlasOps.Features.Observability.LogQueryProvisioning;

public sealed record LogQueryProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);