namespace AtlasOps.Features.Messaging.MessageBrokerRecovery;

public sealed record UpdateMessageBrokerRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);