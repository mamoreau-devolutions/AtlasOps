namespace AtlasOps.Features.Messaging.MessageProducerRecovery;

public sealed record UpdateMessageProducerRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);