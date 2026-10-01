namespace AtlasOps.Features.Hardening.PackageManifest;

public sealed record PackageManifestChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);