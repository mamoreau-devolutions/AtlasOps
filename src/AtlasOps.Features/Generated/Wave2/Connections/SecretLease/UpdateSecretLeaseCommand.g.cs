namespace AtlasOps.Features.Connections.SecretLease;

public sealed record UpdateSecretLeaseCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);