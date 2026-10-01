namespace AtlasOps.Features.Automation.ArtifactPromotion;

public sealed record ArtifactPromotionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);