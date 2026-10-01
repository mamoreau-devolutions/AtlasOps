namespace AtlasOps.Features.Messaging.MessageProducerProvisioning;

public sealed record MessageProducerProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);