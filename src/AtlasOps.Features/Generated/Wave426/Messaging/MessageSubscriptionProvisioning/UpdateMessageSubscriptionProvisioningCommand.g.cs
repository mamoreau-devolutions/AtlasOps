namespace AtlasOps.Features.Messaging.MessageSubscriptionProvisioning;

public sealed record UpdateMessageSubscriptionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);