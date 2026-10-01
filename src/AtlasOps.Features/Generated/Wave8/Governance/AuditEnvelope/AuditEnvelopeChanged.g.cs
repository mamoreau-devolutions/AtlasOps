namespace AtlasOps.Features.Governance.AuditEnvelope;

public sealed record AuditEnvelopeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);