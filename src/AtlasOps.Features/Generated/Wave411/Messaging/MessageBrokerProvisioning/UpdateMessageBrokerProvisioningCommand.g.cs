namespace AtlasOps.Features.Messaging.MessageBrokerProvisioning;

public sealed record UpdateMessageBrokerProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);