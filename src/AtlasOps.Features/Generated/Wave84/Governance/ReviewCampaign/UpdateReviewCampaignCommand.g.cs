namespace AtlasOps.Features.Governance.ReviewCampaign;

public sealed record UpdateReviewCampaignCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);