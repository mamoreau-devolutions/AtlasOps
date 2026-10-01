namespace AtlasOps.Product.Migrations;

public sealed record SettingsSnapshot(
    int Version,
    IReadOnlyDictionary<string, string> Values,
    string Checksum);

public sealed record SettingsMigration(
    string Id,
    int FromVersion,
    int ToVersion,
    Func<IReadOnlyDictionary<string, string>, IReadOnlyDictionary<string, string>> Apply);

public sealed record SettingsMigrationResult(
    bool Succeeded,
    SettingsSnapshot Snapshot,
    IReadOnlyList<string> AppliedMigrations,
    IReadOnlyList<string> Diagnostics);

public sealed class SettingsMigrationService
{
    public SettingsMigrationResult Migrate(
        SettingsSnapshot snapshot,
        int targetVersion,
        IReadOnlyList<SettingsMigration> migrations)
    {
        if (!string.Equals(snapshot.Checksum, CreateChecksum(snapshot.Values), StringComparison.Ordinal))
        {
            return new SettingsMigrationResult(
                false,
                snapshot,
                [],
                ["Settings checksum does not match the supplied values."]);
        }

        SettingsSnapshot current = snapshot;
        List<string> applied = [];
        while (current.Version < targetVersion)
        {
            SettingsMigration? migration = migrations.SingleOrDefault(
                candidate => candidate.FromVersion == current.Version);
            if (migration is null || migration.ToVersion != current.Version + 1)
            {
                return new SettingsMigrationResult(
                    false,
                    current,
                    applied,
                    [$"No contiguous migration starts at version {current.Version}."]);
            }

            IReadOnlyDictionary<string, string> migrated = migration.Apply(current.Values);
            current = new SettingsSnapshot(
                migration.ToVersion,
                migrated,
                CreateChecksum(migrated));
            applied.Add(migration.Id);
        }

        return new SettingsMigrationResult(true, current, applied, []);
    }

    public static string CreateChecksum(IReadOnlyDictionary<string, string> values)
    {
        string canonical = string.Join(
            "\n",
            values
                .OrderBy(static pair => pair.Key, StringComparer.Ordinal)
                .Select(static pair => $"{pair.Key}={pair.Value}"));
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(canonical);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));
    }
}
