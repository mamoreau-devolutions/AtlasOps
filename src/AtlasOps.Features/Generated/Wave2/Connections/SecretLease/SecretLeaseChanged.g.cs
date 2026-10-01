namespace AtlasOps.Features.Connections.SecretLease;

public sealed record SecretLeaseChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);