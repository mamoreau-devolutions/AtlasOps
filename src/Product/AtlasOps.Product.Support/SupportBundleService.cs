namespace AtlasOps.Product.Support;

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public sealed record SupportArtifact(
    string Name,
    string ContentType,
    byte[] Content,
    bool MayContainSecrets);

public sealed record SupportBundleManifestEntry(
    string Name,
    string ContentType,
    long Length,
    string Sha256);

public sealed record SupportBundleResult(
    byte[] Archive,
    IReadOnlyList<SupportBundleManifestEntry> Entries,
    IReadOnlyList<string> ExcludedArtifacts);

public sealed class SupportBundleService
{
    public SupportBundleResult Create(
        IReadOnlyList<SupportArtifact> artifacts,
        IReadOnlySet<string> sensitiveKeys)
    {
        List<SupportBundleManifestEntry> entries = [];
        List<string> excluded = [];
        using MemoryStream output = new();
        using (ZipArchive archive = new(output, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (SupportArtifact artifact in artifacts.OrderBy(static item => item.Name, StringComparer.Ordinal))
            {
                if (!IsSafeName(artifact.Name))
                {
                    excluded.Add(artifact.Name);
                    continue;
                }

                byte[] content = artifact.MayContainSecrets
                    ? Redact(artifact.Content, sensitiveKeys)
                    : artifact.Content;
                WriteEntry(archive, artifact.Name, content);
                entries.Add(new SupportBundleManifestEntry(
                    artifact.Name,
                    artifact.ContentType,
                    content.LongLength,
                    Convert.ToHexString(SHA256.HashData(content))));
            }

            byte[] manifest = JsonSerializer.SerializeToUtf8Bytes(entries);
            WriteEntry(archive, "manifest.json", manifest);
        }

        return new SupportBundleResult(output.ToArray(), entries, excluded);
    }

    private static byte[] Redact(byte[] content, IReadOnlySet<string> sensitiveKeys)
    {
        string text = Encoding.UTF8.GetString(content);
        string[] lines = text.Split(["\r\n", "\n"], StringSplitOptions.None);
        for (int index = 0; index < lines.Length; index++)
        {
            int separator = lines[index].IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            string key = lines[index][..separator].Trim();
            if (sensitiveKeys.Contains(key))
            {
                lines[index] = $"{key}=[REDACTED]";
            }
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
    }

    private static bool IsSafeName(string name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               !Path.IsPathRooted(name) &&
               !name.Split('/', '\\').Any(static segment => segment is "." or "..");
    }

    private static void WriteEntry(ZipArchive archive, string name, byte[] content)
    {
        ZipArchiveEntry entry = archive.CreateEntry(name, CompressionLevel.Optimal);
        using Stream stream = entry.Open();
        stream.Write(content);
    }
}
