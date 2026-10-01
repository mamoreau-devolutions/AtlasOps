namespace AtlasOps.Features.FinOps.FinOpsReportProvisioning;

public sealed record FinOpsReportProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);