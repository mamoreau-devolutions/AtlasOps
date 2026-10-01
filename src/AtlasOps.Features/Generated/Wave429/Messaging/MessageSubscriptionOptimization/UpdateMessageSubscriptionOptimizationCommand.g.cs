namespace AtlasOps.Features.Messaging.MessageSubscriptionOptimization;

public sealed record UpdateMessageSubscriptionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);