namespace AtlasOps.Features.Incidents.PostmortemReview;

public sealed record PostmortemReviewChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);