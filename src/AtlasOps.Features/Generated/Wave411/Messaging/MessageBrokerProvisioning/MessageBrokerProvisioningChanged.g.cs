namespace AtlasOps.Features.Messaging.MessageBrokerProvisioning;

public sealed record MessageBrokerProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);