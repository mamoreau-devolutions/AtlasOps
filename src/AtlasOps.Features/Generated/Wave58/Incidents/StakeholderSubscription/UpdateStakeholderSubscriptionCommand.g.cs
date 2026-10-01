namespace AtlasOps.Features.Incidents.StakeholderSubscription;

public sealed record UpdateStakeholderSubscriptionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);