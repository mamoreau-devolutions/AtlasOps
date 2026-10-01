namespace AtlasOps.Features.Messaging.MessageTopicRecovery;

public sealed record UpdateMessageTopicRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);