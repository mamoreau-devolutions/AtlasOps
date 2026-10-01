namespace AtlasOps.Features.FinOps.CloudInvoiceGovernance;

public sealed record CloudInvoiceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);