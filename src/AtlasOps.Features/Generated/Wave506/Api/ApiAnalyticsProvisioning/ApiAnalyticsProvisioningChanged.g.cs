namespace AtlasOps.Features.Api.ApiAnalyticsProvisioning;

public sealed record ApiAnalyticsProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);