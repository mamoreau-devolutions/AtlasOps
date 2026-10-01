namespace AtlasOps.Features.Incidents.PostmortemReview;

public sealed record UpdatePostmortemReviewCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);