namespace AtlasOps.Features.Messaging.MessageSubscriptionGovernance;

public sealed record UpdateMessageSubscriptionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);