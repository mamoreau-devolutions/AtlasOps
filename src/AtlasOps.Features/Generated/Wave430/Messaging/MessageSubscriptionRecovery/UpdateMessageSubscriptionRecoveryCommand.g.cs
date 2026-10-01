namespace AtlasOps.Features.Messaging.MessageSubscriptionRecovery;

public sealed record UpdateMessageSubscriptionRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);