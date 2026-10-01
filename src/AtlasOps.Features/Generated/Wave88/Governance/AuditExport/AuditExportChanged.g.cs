namespace AtlasOps.Features.Governance.AuditExport;

public sealed record AuditExportChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);