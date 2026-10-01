namespace AtlasOps.Features.Messaging.MessageQueueOptimization;

public sealed record UpdateMessageQueueOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);