namespace AtlasOps.Features.Messaging.MessageQueueProvisioning;

public sealed record UpdateMessageQueueProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);