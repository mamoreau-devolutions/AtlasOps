namespace AtlasOps.Features.Messaging.MessageTopicProvisioning;

public sealed record MessageTopicProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);