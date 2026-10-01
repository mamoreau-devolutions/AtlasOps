namespace AtlasOps.Extensions.Runtime;

using AtlasOps.Extensions.Contracts;

public sealed class ExtensionMigrationService
{
    public ExtensionMigrationResult Migrate(
        int currentVersion,
        int targetVersion,
        IReadOnlyDictionary<string, string> currentValues,
        IReadOnlyList<ExtensionConfigurationMigration> migrations)
    {
        Dictionary<string, string> values = new(currentValues, StringComparer.OrdinalIgnoreCase);
        List<string> applied = [];
        List<string> diagnostics = [];
        int version = currentVersion;

        while (version < targetVersion)
        {
            ExtensionConfigurationMigration? migration = migrations.SingleOrDefault(
                candidate => candidate.FromVersion == version);
            if (migration is null)
            {
                diagnostics.Add($"No migration starts at configuration version {version}.");
                return new ExtensionMigrationResult(false, version, values, applied, diagnostics);
            }

            string[] missingKeys = migration.RequiredKeys
                .Where(key => !values.ContainsKey(key))
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (missingKeys.Length > 0)
            {
                diagnostics.Add(
                    $"Migration '{migration.Id}' requires missing keys: {string.Join(", ", missingKeys)}.");
                return new ExtensionMigrationResult(false, version, values, applied, diagnostics);
            }

            foreach (string removedKey in migration.RemovedKeys)
            {
                values.Remove(removedKey);
            }

            values["schemaVersion"] = migration.ToVersion.ToString(
                System.Globalization.CultureInfo.InvariantCulture);
            applied.Add(migration.Id);
            version = migration.ToVersion;
        }

        if (version != targetVersion)
        {
            diagnostics.Add("Migration chain passed the requested target version.");
            return new ExtensionMigrationResult(false, version, values, applied, diagnostics);
        }

        return new ExtensionMigrationResult(true, version, values, applied, diagnostics);
    }
}
