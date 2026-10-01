namespace AtlasOps.ReferenceData.Infrastructure;

using System.Security.Cryptography;
using System.Text;

public static class ReferenceDataIntegrity
{
    public static async Task<IReadOnlyList<ReferenceValidationIssue>> VerifyAsync(
        string directory,
        ReferencePackManifest manifest,
        CancellationToken cancellationToken = default)
    {
        List<ReferenceValidationIssue> issues = [];
        foreach (ReferenceManifestFile item in manifest.Files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string path = Path.Combine(directory, item.Path.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(path))
            {
                issues.Add(new ReferenceValidationIssue(
                    "Error",
                    "PACK-FILE-MISSING",
                    "A file listed in the source manifest is missing.",
                    item.Path));
                continue;
            }

            FileInfo info = new(path);
            if (info.Length != item.Bytes)
            {
                issues.Add(new ReferenceValidationIssue(
                    "Error",
                    "PACK-SIZE-MISMATCH",
                    $"Expected {item.Bytes:N0} bytes but found {info.Length:N0}.",
                    item.Path));
                continue;
            }

            string hash = await ComputeFileHashAsync(path, cancellationToken);
            if (!hash.Equals(item.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                issues.Add(new ReferenceValidationIssue(
                    "Error",
                    "PACK-HASH-MISMATCH",
                    "The file SHA-256 does not match the pinned source manifest.",
                    item.Path));
            }
        }

        return issues;
    }

    public static async Task<string> ComputeDirectoryFingerprintAsync(
        string directory,
        CancellationToken cancellationToken = default)
    {
        using IncrementalHash directoryHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (string path in Directory
            .EnumerateFiles(directory, "*", SearchOption.AllDirectories)
            .Where(static path => !Path.GetFileName(path).Equals("manifest.json", StringComparison.OrdinalIgnoreCase))
            .Order(StringComparer.Ordinal))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string relativePath = Path.GetRelativePath(directory, path).Replace('\\', '/');
            byte[] nameBytes = Encoding.UTF8.GetBytes(relativePath);
            directoryHash.AppendData(nameBytes);
            directoryHash.AppendData([0]);
            string fileHash = await ComputeFileHashAsync(path, cancellationToken);
            directoryHash.AppendData(Convert.FromHexString(fileHash));
        }

        return Convert.ToHexString(directoryHash.GetHashAndReset()).ToLowerInvariant();
    }

    private static async Task<string> ComputeFileHashAsync(
        string path,
        CancellationToken cancellationToken)
    {
        await using FileStream stream = File.OpenRead(path);
        byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}