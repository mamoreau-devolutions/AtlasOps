namespace AtlasOps.Extensions.Runtime;

using AtlasOps.Extensions.Contracts;

public sealed class ExtensionManifestService
{
    public ExtensionValidationResult Validate(
        ExtensionManifest manifest,
        IReadOnlySet<string> knownPermissions)
    {
        List<string> diagnostics = [];
        if (string.IsNullOrWhiteSpace(manifest.Id) ||
            manifest.Id.Any(static character => !char.IsLetterOrDigit(character) && character is not '.' and not '-'))
        {
            diagnostics.Add("Extension ID must use letters, digits, periods, and hyphens.");
        }

        if (!Version.TryParse(manifest.Version, out Version? extensionVersion) ||
            extensionVersion.Major < 1)
        {
            diagnostics.Add("Extension version must be a semantic version with major version one or later.");
        }

        HashSet<string> permissionIds = new(StringComparer.OrdinalIgnoreCase);
        foreach (ExtensionPermission permission in manifest.Permissions)
        {
            if (!knownPermissions.Contains(permission.Id))
            {
                diagnostics.Add($"Permission '{permission.Id}' is not recognized.");
            }

            if (!permissionIds.Add(permission.Id))
            {
                diagnostics.Add($"Permission '{permission.Id}' is duplicated.");
            }

            if (permission.Sensitive && string.IsNullOrWhiteSpace(permission.Justification))
            {
                diagnostics.Add($"Sensitive permission '{permission.Id}' requires a justification.");
            }
        }

        HashSet<string> capabilityIds = new(StringComparer.OrdinalIgnoreCase);
        foreach (ExtensionCapability capability in manifest.Capabilities)
        {
            if (!capabilityIds.Add(capability.Id))
            {
                diagnostics.Add($"Capability '{capability.Id}' is duplicated.");
            }

            if (capability.Operations.Count == 0)
            {
                diagnostics.Add($"Capability '{capability.Id}' must expose at least one operation.");
            }
        }

        ValidateMigrations(manifest.Migrations, diagnostics);
        return new ExtensionValidationResult(diagnostics.Count == 0, diagnostics);
    }

    public ExtensionCompatibilityResult EvaluateCompatibility(
        ExtensionManifest manifest,
        Version hostVersion)
    {
        List<string> diagnostics = [];
        if (!Version.TryParse(manifest.MinimumHostVersion, out Version? minimum))
        {
            diagnostics.Add("Minimum host version is invalid.");
        }
        else if (hostVersion < minimum)
        {
            diagnostics.Add($"Host {hostVersion} is older than required version {minimum}.");
        }

        if (!Version.TryParse(manifest.MaximumHostVersion, out Version? maximum))
        {
            diagnostics.Add("Maximum host version is invalid.");
        }
        else if (hostVersion > maximum)
        {
            diagnostics.Add($"Host {hostVersion} is newer than supported version {maximum}.");
        }

        return new ExtensionCompatibilityResult(diagnostics.Count == 0, diagnostics);
    }

    private static void ValidateMigrations(
        IReadOnlyList<ExtensionConfigurationMigration> migrations,
        List<string> diagnostics)
    {
        int? expectedVersion = null;
        HashSet<string> migrationIds = new(StringComparer.OrdinalIgnoreCase);
        foreach (ExtensionConfigurationMigration migration in migrations.OrderBy(static item => item.FromVersion))
        {
            if (!migrationIds.Add(migration.Id))
            {
                diagnostics.Add($"Migration '{migration.Id}' is duplicated.");
            }

            if (migration.ToVersion != migration.FromVersion + 1)
            {
                diagnostics.Add($"Migration '{migration.Id}' must advance exactly one version.");
            }

            if (expectedVersion is not null && migration.FromVersion != expectedVersion)
            {
                diagnostics.Add($"Migration chain skips version {expectedVersion}.");
            }

            expectedVersion = migration.ToVersion;
        }
    }
}
