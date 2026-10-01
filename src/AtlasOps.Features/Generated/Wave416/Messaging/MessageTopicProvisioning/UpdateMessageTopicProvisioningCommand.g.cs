namespace AtlasOps.Features.Messaging.MessageTopicProvisioning;

public sealed record UpdateMessageTopicProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);