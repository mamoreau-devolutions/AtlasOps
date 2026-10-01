namespace AtlasOps.Features.Messaging.MessageRetentionProvisioning;

public sealed record UpdateMessageRetentionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);