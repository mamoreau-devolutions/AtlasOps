namespace AtlasOps.Features.Messaging.MessageConsumerProvisioning;

public sealed record MessageConsumerProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);