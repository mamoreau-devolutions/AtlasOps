namespace AtlasOps.Features.Connections.HostKeyVerification;

public sealed record HostKeyVerificationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);