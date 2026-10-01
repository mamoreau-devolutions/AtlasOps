namespace AtlasOps.Features.Analytics.AnalyticsSubscription;

public sealed record AnalyticsSubscriptionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);