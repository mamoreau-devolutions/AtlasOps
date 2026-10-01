namespace AtlasOps.Features.Messaging.MessageConsumerRecovery;

public sealed record UpdateMessageConsumerRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);