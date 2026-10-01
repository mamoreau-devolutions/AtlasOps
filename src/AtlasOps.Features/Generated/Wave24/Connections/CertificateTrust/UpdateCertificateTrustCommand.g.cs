namespace AtlasOps.Features.Connections.CertificateTrust;

public sealed record UpdateCertificateTrustCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);