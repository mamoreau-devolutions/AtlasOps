namespace AtlasOps.Features.Messaging.MessageQueueProvisioning;

public sealed record MessageQueueProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);