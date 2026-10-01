namespace AtlasOps.Features.Messaging.MessageProducerOptimization;

public sealed record UpdateMessageProducerOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);