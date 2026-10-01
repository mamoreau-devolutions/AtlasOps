namespace AtlasOps.Features.FinOps.CloudInvoiceProvisioning;

public sealed record CloudInvoiceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);