namespace AtlasOps.Features.Messaging.MessageDeadLetterProvisioning;

public sealed record MessageDeadLetterProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);