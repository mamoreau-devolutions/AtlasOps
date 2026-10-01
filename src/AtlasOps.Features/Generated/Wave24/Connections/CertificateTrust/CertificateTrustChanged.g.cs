namespace AtlasOps.Features.Connections.CertificateTrust;

public sealed record CertificateTrustChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);