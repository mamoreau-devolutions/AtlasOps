namespace AtlasOps.Features.Cloud.GcpProjectProvisioning;

public sealed record GcpProjectProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);