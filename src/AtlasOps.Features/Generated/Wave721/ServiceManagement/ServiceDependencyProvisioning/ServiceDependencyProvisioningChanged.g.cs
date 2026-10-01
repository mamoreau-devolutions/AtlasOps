namespace AtlasOps.Features.ServiceManagement.ServiceDependencyProvisioning;

public sealed record ServiceDependencyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);