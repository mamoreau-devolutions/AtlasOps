namespace AtlasOps.Features.Messaging.MessageReplayProvisioning;

public sealed record UpdateMessageReplayProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);