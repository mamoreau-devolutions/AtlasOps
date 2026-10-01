namespace AtlasOps.Features.ServiceManagement.ServiceScorecardProvisioning;

public sealed record ServiceScorecardProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);