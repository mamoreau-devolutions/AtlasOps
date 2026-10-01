namespace AtlasOps.Features.Incidents.StakeholderSubscription;

public sealed record StakeholderSubscriptionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);