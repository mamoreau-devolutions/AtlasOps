namespace AtlasOps.Features.Connections.CredentialReference;

public sealed record CredentialReferenceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);