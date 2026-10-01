namespace AtlasOps.Features.Analytics.AnalyticsSubscription;

public sealed record UpdateAnalyticsSubscriptionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);