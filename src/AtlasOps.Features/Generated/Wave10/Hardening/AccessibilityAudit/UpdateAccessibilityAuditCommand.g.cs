namespace AtlasOps.Features.Hardening.AccessibilityAudit;

public sealed record UpdateAccessibilityAuditCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);