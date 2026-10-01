namespace AtlasOps.Features.Messaging.MessageRetentionProvisioning;

public sealed record MessageRetentionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);