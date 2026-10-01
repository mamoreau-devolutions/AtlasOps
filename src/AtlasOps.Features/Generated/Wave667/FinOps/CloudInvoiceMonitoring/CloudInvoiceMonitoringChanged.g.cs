namespace AtlasOps.Features.FinOps.CloudInvoiceMonitoring;

public sealed record CloudInvoiceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);