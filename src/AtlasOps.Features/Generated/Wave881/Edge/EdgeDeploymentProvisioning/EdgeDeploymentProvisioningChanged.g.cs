namespace AtlasOps.Features.Edge.EdgeDeploymentProvisioning;

public sealed record EdgeDeploymentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);