namespace AtlasOps.Features.FinOps.CloudInvoiceOptimization;

public sealed record CloudInvoiceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);