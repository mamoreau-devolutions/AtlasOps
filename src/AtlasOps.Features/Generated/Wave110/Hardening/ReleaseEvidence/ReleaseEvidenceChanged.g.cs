namespace AtlasOps.Features.Hardening.ReleaseEvidence;

public sealed record ReleaseEvidenceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);