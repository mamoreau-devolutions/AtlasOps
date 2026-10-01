namespace AtlasOps.Features.Messaging.MessageConsumerProvisioning;

public sealed record UpdateMessageConsumerProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);