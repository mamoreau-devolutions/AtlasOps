namespace AtlasOps.Features.Governance.ReviewCampaign;

public sealed record ReviewCampaignChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);