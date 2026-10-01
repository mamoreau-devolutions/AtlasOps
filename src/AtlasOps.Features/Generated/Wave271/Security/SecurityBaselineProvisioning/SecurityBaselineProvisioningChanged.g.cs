namespace AtlasOps.Features.Security.SecurityBaselineProvisioning;

public sealed record SecurityBaselineProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);