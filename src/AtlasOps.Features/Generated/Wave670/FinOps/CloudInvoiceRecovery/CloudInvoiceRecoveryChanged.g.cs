namespace AtlasOps.Features.FinOps.CloudInvoiceRecovery;

public sealed record CloudInvoiceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);