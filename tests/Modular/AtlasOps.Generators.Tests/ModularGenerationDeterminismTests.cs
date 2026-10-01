namespace AtlasOps.Generators.Tests;

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

[TestClass]
public sealed class ModularGenerationDeterminismTests
{
    private string? _temporaryRoot;

    [TestCleanup]
    public void Cleanup()
    {
        if (this._temporaryRoot is not null && Directory.Exists(this._temporaryRoot))
        {
            Directory.Delete(this._temporaryRoot, recursive: true);
        }
    }

    [TestMethod]
    public void Manifest_DeclaresTheExactBoundedArchitecture()
    {
        string atlasOpsRoot = FindAtlasOpsRoot();
        using JsonDocument manifest = JsonDocument.Parse(
            File.ReadAllText(Path.Combine(atlasOpsRoot, "config", "atlasops-modules.json")));
        JsonElement root = manifest.RootElement;

        Assert.AreEqual(1, root.GetProperty("schemaVersion").GetInt32());
        Assert.AreEqual(100, root.GetProperty("targetProjectCount").GetInt32());
        Assert.AreEqual(30, root.GetProperty("capabilitiesPerDomain").GetInt32());
        Assert.AreEqual(18, root.GetProperty("domains").GetArrayLength());
        Assert.AreEqual(14, root.GetProperty("platformProjects").GetArrayLength());
        Assert.AreEqual(10, root.GetProperty("adapters").GetArrayLength());
        Assert.AreEqual(6, root.GetProperty("tools").GetArrayLength());
        Assert.AreEqual(9, root.GetProperty("testProjects").GetArrayLength());
    }

    [TestMethod]
    public async Task Generator_IdenticalInputTwice_ProducesByteIdenticalIdempotentOutput()
    {
        string sourceRoot = FindAtlasOpsRoot();
        this._temporaryRoot = Path.Combine(Path.GetTempPath(), $"atlasops-generation-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this._temporaryRoot, "config"));
        Directory.CreateDirectory(Path.Combine(this._temporaryRoot, "scripts"));
        File.Copy(
            Path.Combine(sourceRoot, "config", "atlasops-modules.json"),
            Path.Combine(this._temporaryRoot, "config", "atlasops-modules.json"));
        string script = Path.Combine(this._temporaryRoot, "scripts", "generate_modular.py");
        File.Copy(Path.Combine(sourceRoot, "scripts", "generate_modular.py"), script);

        ProcessResult firstRun = await RunGeneratorAsync(script, this._temporaryRoot);
        IReadOnlyDictionary<string, string> firstSnapshot = SnapshotGeneratedFiles(this._temporaryRoot);
        ProcessResult secondRun = await RunGeneratorAsync(script, this._temporaryRoot);
        IReadOnlyDictionary<string, string> secondSnapshot = SnapshotGeneratedFiles(this._temporaryRoot);

        Assert.AreEqual(0, firstRun.ExitCode, firstRun.StandardError);
        Assert.AreEqual(0, secondRun.ExitCode, secondRun.StandardError);
        Assert.Contains("Generated 93 modular projects across 18 domains.", firstRun.StandardOutput);
        Assert.Contains("Generated 93 modular projects across 18 domains.", secondRun.StandardOutput);
        Assert.HasCount(93, firstSnapshot.Keys.Where(static path => path.EndsWith(".csproj", StringComparison.Ordinal)));
        Assert.HasCount(firstSnapshot.Count, secondSnapshot);
        CollectionAssert.AreEquivalent(firstSnapshot.Keys.ToArray(), secondSnapshot.Keys.ToArray());
        foreach ((string path, string hash) in firstSnapshot)
        {
            Assert.AreEqual(hash, secondSnapshot[path], $"Generated content changed on the second run: {path}");
        }

        string compositionDirectory = Path.Combine(
            this._temporaryRoot,
            "src",
            "Modular",
            "Platform",
            "AtlasOps.Composition");
        string[] catalogFiles = Directory.GetFiles(compositionDirectory, "AtlasOpsModuleCatalog*.cs");
        Assert.HasCount(1, catalogFiles);
        string catalog = File.ReadAllText(catalogFiles[0]);
        Assert.IsTrue(catalog.Contains("\r\n", StringComparison.Ordinal));
        Assert.IsLessThan(
            catalog.IndexOf("Compliance", StringComparison.Ordinal),
            catalog.IndexOf("Workspaces", StringComparison.Ordinal));
    }

    [TestMethod]
    public async Task Generator_DuplicateManifestEntry_FailsBeforeReportingAValidProjectSet()
    {
        string sourceRoot = FindAtlasOpsRoot();
        this._temporaryRoot = Path.Combine(Path.GetTempPath(), $"atlasops-invalid-generation-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this._temporaryRoot, "config"));
        Directory.CreateDirectory(Path.Combine(this._temporaryRoot, "scripts"));
        string sourceManifest = File.ReadAllText(Path.Combine(sourceRoot, "config", "atlasops-modules.json"));
        using JsonDocument document = JsonDocument.Parse(sourceManifest);
        Dictionary<string, object?> manifest = JsonSerializer.Deserialize<Dictionary<string, object?>>(sourceManifest)!;
        List<string> adapters = document.RootElement.GetProperty("adapters")
            .EnumerateArray()
            .Select(static item => item.GetString()!)
            .ToList();
        adapters.Add(adapters[0]);
        manifest["adapters"] = adapters;
        File.WriteAllText(
            Path.Combine(this._temporaryRoot, "config", "atlasops-modules.json"),
            JsonSerializer.Serialize(manifest));
        string script = Path.Combine(this._temporaryRoot, "scripts", "generate_modular.py");
        File.Copy(Path.Combine(sourceRoot, "scripts", "generate_modular.py"), script);

        ProcessResult result = await RunGeneratorAsync(script, this._temporaryRoot);

        Assert.AreNotEqual(0, result.ExitCode);
        Assert.Contains("Expected 93 generated projects but produced 94.", result.StandardError);
        Assert.DoesNotContain("Generated 93 modular projects", result.StandardOutput);
    }

    private static async Task<ProcessResult> RunGeneratorAsync(string script, string root)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "python",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add(script);
        startInfo.ArgumentList.Add("--repository-root");
        startInfo.ArgumentList.Add(root);
        startInfo.ArgumentList.Add("--skip-solution-update");
        using Process process = Process.Start(startInfo)
            ?? throw new AssertFailedException("The Python generator process could not be started.");
        string standardOutput = await process.StandardOutput.ReadToEndAsync();
        string standardError = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return new(process.ExitCode, standardOutput, standardError);
    }

    private static IReadOnlyDictionary<string, string> SnapshotGeneratedFiles(string root)
    {
        string[] boundaries =
        [
            Path.Combine(root, "src", "Modular"),
            Path.Combine(root, "tests", "Modular"),
        ];
        return boundaries
            .SelectMany(boundary => Directory.GetFiles(boundary, "*", SearchOption.AllDirectories))
            .OrderBy(static path => path, StringComparer.Ordinal)
            .ToDictionary(
                path => Path.GetRelativePath(root, path).Replace('\\', '/'),
                path => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))),
                StringComparer.Ordinal);
    }

    private static string FindAtlasOpsRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "config", "atlasops-modules.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        Assert.Fail("Could not locate the AtlasOps generator manifest.");
        return string.Empty;
    }

    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}