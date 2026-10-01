namespace AtlasOps.Features.Api.ApiDeploymentProvisioning;

public sealed record ApiDeploymentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);