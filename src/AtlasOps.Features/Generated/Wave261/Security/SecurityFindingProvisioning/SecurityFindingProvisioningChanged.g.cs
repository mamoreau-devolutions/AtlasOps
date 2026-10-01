namespace AtlasOps.Features.Security.SecurityFindingProvisioning;

public sealed record SecurityFindingProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);