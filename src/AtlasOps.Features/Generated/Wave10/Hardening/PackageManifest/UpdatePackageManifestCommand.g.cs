namespace AtlasOps.Features.Hardening.PackageManifest;

public sealed record UpdatePackageManifestCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);