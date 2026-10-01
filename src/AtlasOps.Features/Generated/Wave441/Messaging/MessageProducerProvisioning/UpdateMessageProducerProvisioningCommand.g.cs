namespace AtlasOps.Features.Messaging.MessageProducerProvisioning;

public sealed record UpdateMessageProducerProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);