namespace AtlasOps.Features.Messaging.MessageSubscriptionProvisioning;

public sealed record MessageSubscriptionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);