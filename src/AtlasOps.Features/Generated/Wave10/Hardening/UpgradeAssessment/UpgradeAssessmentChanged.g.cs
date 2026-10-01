namespace AtlasOps.Features.Hardening.UpgradeAssessment;

public sealed record UpgradeAssessmentChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);