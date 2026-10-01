namespace AtlasOps.Features.Messaging.MessageReplayOptimization;

public sealed record UpdateMessageReplayOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);