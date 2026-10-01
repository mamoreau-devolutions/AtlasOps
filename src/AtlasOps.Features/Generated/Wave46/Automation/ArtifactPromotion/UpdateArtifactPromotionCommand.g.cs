namespace AtlasOps.Features.Automation.ArtifactPromotion;

public sealed record UpdateArtifactPromotionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);