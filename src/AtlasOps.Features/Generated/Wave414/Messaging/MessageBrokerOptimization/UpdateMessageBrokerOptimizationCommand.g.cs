namespace AtlasOps.Features.Messaging.MessageBrokerOptimization;

public sealed record UpdateMessageBrokerOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);