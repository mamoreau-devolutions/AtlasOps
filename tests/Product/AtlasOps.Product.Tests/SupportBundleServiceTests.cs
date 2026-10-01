namespace AtlasOps.Product.Tests;

using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using AtlasOps.Product.Support;

[TestClass]
public sealed class SupportBundleServiceTests
{
    [TestMethod]
    public void Create_OrdersSafeEntriesAndManifestWithConcreteHashes()
    {
        SupportBundleService service = new();
        SupportArtifact zeta = new("zeta.txt", "text/plain", "zeta"u8.ToArray(), false);
        SupportArtifact alpha = new("alpha.txt", "text/plain", "alpha"u8.ToArray(), false);

        SupportBundleResult result = service.Create([zeta, alpha], new HashSet<string>());

        CollectionAssert.AreEqual(new[] { "alpha.txt", "zeta.txt" }, result.Entries.Select(static entry => entry.Name).ToArray());
        Assert.AreEqual(5L, result.Entries[0].Length);
        Assert.AreEqual(
            Convert.ToHexString(SHA256.HashData(alpha.Content)),
            result.Entries[0].Sha256);
        using MemoryStream stream = new(result.Archive);
        using ZipArchive archive = new(stream, ZipArchiveMode.Read);
        CollectionAssert.AreEqual(
            new[] { "alpha.txt", "zeta.txt", "manifest.json" },
            archive.Entries.Select(static entry => entry.FullName).ToArray());
        SupportBundleManifestEntry[] manifest = ReadManifest(archive);
        CollectionAssert.AreEqual(result.Entries.ToArray(), manifest);
    }

    [TestMethod]
    public void Create_RedactsOnlySensitiveKeysAndHashesRedactedContent()
    {
        SupportBundleService service = new();
        byte[] content = Encoding.UTF8.GetBytes("token=secret\nsafe=value\n token = another");
        SupportArtifact artifact = new("config.txt", "text/plain", content, true);

        SupportBundleResult result = service.Create([artifact], new HashSet<string>(["token"]));

        using MemoryStream stream = new(result.Archive);
        using ZipArchive archive = new(stream, ZipArchiveMode.Read);
        string redacted = ReadText(archive.GetEntry("config.txt")!);
        string expected = string.Join(
            Environment.NewLine,
            "token=[REDACTED]",
            "safe=value",
            "token=[REDACTED]");
        Assert.AreEqual(expected, redacted);
        Assert.AreEqual(
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(expected))),
            result.Entries.Single().Sha256);
        Assert.DoesNotContain("secret", redacted);
        Assert.Contains("safe=value", redacted);
    }

    [TestMethod]
    public void Create_ExcludesBlankRootedAndTraversalNamesWithoutArchiveEntries()
    {
        SupportBundleService service = new();
        SupportArtifact[] artifacts =
        [
            new("", "text/plain", [1], false),
            new(Path.GetFullPath("rooted.txt"), "text/plain", [2], false),
            new("../parent.txt", "text/plain", [3], false),
            new("folder/./child.txt", "text/plain", [4], false),
            new("folder/safe.txt", "text/plain", [5], false),
        ];

        SupportBundleResult result = service.Create(artifacts, new HashSet<string>());

        Assert.HasCount(4, result.ExcludedArtifacts);
        CollectionAssert.AreEqual(
            artifacts.Take(4).Select(static artifact => artifact.Name).Order(StringComparer.Ordinal).ToArray(),
            result.ExcludedArtifacts.Order(StringComparer.Ordinal).ToArray());
        Assert.HasCount(1, result.Entries);
        Assert.AreEqual("folder/safe.txt", result.Entries[0].Name);
        using MemoryStream stream = new(result.Archive);
        using ZipArchive archive = new(stream, ZipArchiveMode.Read);
        Assert.IsTrue(archive.Entries.All(static entry =>
            !Path.IsPathRooted(entry.FullName) &&
            !entry.FullName.Split('/', '\\').Any(static segment => segment is "." or "..")));
    }

    private static SupportBundleManifestEntry[] ReadManifest(ZipArchive archive)
    {
        string json = ReadText(archive.GetEntry("manifest.json")!);
        return JsonSerializer.Deserialize<SupportBundleManifestEntry[]>(json)!;
    }

    private static string ReadText(ZipArchiveEntry entry)
    {
        using StreamReader reader = new(entry.Open(), Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
