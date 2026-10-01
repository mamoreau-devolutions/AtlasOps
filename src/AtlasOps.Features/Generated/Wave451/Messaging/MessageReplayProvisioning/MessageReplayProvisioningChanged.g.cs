namespace AtlasOps.Features.Messaging.MessageReplayProvisioning;

public sealed record MessageReplayProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);