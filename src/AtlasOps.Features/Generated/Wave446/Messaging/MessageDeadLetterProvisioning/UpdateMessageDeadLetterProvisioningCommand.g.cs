namespace AtlasOps.Features.Messaging.MessageDeadLetterProvisioning;

public sealed record UpdateMessageDeadLetterProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);