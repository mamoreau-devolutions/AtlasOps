namespace AtlasOps.Features.Hardening.AccessibilityAudit;

public sealed record AccessibilityAuditChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);