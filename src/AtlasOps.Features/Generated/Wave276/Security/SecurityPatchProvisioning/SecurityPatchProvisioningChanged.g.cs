namespace AtlasOps.Features.Security.SecurityPatchProvisioning;

public sealed record SecurityPatchProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);