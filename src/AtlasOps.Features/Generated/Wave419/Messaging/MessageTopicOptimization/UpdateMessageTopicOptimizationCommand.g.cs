namespace AtlasOps.Features.Messaging.MessageTopicOptimization;

public sealed record UpdateMessageTopicOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);