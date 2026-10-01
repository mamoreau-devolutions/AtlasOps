namespace AtlasOps.Features.Governance.AuditExport;

public sealed record UpdateAuditExportCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);