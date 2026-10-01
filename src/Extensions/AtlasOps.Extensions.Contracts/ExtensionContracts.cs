namespace AtlasOps.Extensions.Contracts;

public sealed record ExtensionPermission(
    string Id,
    string DisplayName,
    bool Sensitive,
    string Justification);

public sealed record ExtensionCapability(
    string Id,
    string Version,
    IReadOnlyList<string> Operations);

public sealed record ExtensionManifest(
    string Id,
    string DisplayName,
    string Version,
    string MinimumHostVersion,
    string MaximumHostVersion,
    IReadOnlyList<ExtensionPermission> Permissions,
    IReadOnlyList<ExtensionCapability> Capabilities,
    IReadOnlyList<ExtensionConfigurationMigration> Migrations);

public sealed record ExtensionConfigurationMigration(
    int FromVersion,
    int ToVersion,
    string Id,
    IReadOnlyList<string> RequiredKeys,
    IReadOnlyList<string> RemovedKeys);

public sealed record ExtensionValidationResult(
    bool Valid,
    IReadOnlyList<string> Diagnostics);

public sealed record ExtensionCompatibilityResult(
    bool Compatible,
    IReadOnlyList<string> Diagnostics);

public sealed record ExtensionMigrationResult(
    bool Succeeded,
    int Version,
    IReadOnlyDictionary<string, string> Values,
    IReadOnlyList<string> AppliedMigrations,
    IReadOnlyList<string> Diagnostics);
