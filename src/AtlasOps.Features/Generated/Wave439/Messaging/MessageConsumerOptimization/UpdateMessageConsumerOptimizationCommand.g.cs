namespace AtlasOps.Features.Messaging.MessageConsumerOptimization;

public sealed record UpdateMessageConsumerOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);