namespace AtlasOps.Features.Hardening.LocalizationAudit;

public sealed record LocalizationAuditChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);