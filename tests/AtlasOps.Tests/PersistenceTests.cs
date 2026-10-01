namespace AtlasOps.Tests;

using System.Text.Json;

using AtlasOps.Core;

[TestClass]
public sealed class PersistenceTests
{
    private string temporaryRoot = null!;

    [TestInitialize]
    public void Initialize()
    {
        this.temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            "AtlasOps.Tests",
            Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(this.temporaryRoot))
        {
            Directory.Delete(this.temporaryRoot, recursive: true);
        }
    }

    [TestMethod]
    public async Task FileStore_MissingFile_ReturnsEmptyAndCreatesDirectory()
    {
        FileAtlasOpsStore store = new(this.temporaryRoot);

        IReadOnlyList<AtlasOpsProject> models = await store.LoadAsync<AtlasOpsProject>();

        Assert.AreEqual("Local JSON", store.Kind);
        Assert.IsTrue(Directory.Exists(this.temporaryRoot));
        Assert.HasCount(0, models);
        Assert.IsFalse(File.Exists(Path.Combine(this.temporaryRoot, "AtlasOpsProject.json")));
    }

    [TestMethod]
    public async Task FileStore_SaveLoad_RoundTripsConcreteValuesAndRemovesTemporaryFile()
    {
        FileAtlasOpsStore store = new(this.temporaryRoot);
        AtlasOpsProject[] expected =
        [
            new()
            {
                Id = "project-a",
                Name = "Alpha",
                Owner = "Platform",
                Priority = 4,
                IsPinned = true,
                UpdatedAt = ModelTestData.FixedTimestamp,
            },
            new()
            {
                Id = "project-b",
                Name = "Beta",
                Owner = "Operations",
                Priority = 8,
                IsPinned = false,
                UpdatedAt = ModelTestData.FixedTimestamp.AddDays(1),
            },
        ];

        await store.SaveAsync(expected);
        IReadOnlyList<AtlasOpsProject> actual = await store.LoadAsync<AtlasOpsProject>();

        Assert.HasCount(2, actual);
        Assert.AreEqual("project-a", actual[0].Id);
        Assert.AreEqual("Alpha", actual[0].Name);
        Assert.AreEqual("Platform", actual[0].Owner);
        Assert.AreEqual(4, actual[0].Priority);
        Assert.IsTrue(actual[0].IsPinned);
        Assert.AreEqual(ModelTestData.FixedTimestamp, actual[0].UpdatedAt);
        Assert.AreEqual("project-b", actual[1].Id);
        Assert.AreEqual("Beta", actual[1].Name);
        Assert.IsFalse(File.Exists(Path.Combine(this.temporaryRoot, "AtlasOpsProject.json.tmp")));
    }

    [TestMethod]
    public async Task FileStore_SecondSave_OverwritesPriorModels()
    {
        FileAtlasOpsStore store = new(this.temporaryRoot);
        await store.SaveAsync(
        [
            new AtlasOpsProject { Id = "old-1", Name = "Old one" },
            new AtlasOpsProject { Id = "old-2", Name = "Old two" },
        ]);

        await store.SaveAsync(
        [
            new AtlasOpsProject { Id = "replacement", Name = "Replacement" },
        ]);
        IReadOnlyList<AtlasOpsProject> actual = await store.LoadAsync<AtlasOpsProject>();

        AtlasOpsProject replacement = actual.Single();
        Assert.AreEqual("replacement", replacement.Id);
        Assert.AreEqual("Replacement", replacement.Name);
        Assert.IsFalse(actual.Any(project => project.Id.StartsWith("old-", StringComparison.Ordinal)));

        await store.SaveAsync(Array.Empty<AtlasOpsProject>());
        Assert.HasCount(0, await store.LoadAsync<AtlasOpsProject>());
    }

    [TestMethod]
    public async Task FileStore_PreCanceledLoadAndSave_ThrowOperationCanceledException()
    {
        FileAtlasOpsStore store = new(this.temporaryRoot);
        await store.SaveAsync([new AtlasOpsProject { Id = "existing", Name = "Existing" }]);
        using CancellationTokenSource source = new();
        source.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => store.LoadAsync<AtlasOpsProject>(source.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => store.SaveAsync(
                [new AtlasOpsProject { Id = "new", Name = "New" }],
                source.Token));

        IReadOnlyList<AtlasOpsProject> persisted = await store.LoadAsync<AtlasOpsProject>();
        AtlasOpsProject existing = persisted.Single();
        Assert.AreEqual("existing", existing.Id);
        Assert.AreEqual("Existing", existing.Name);
    }

    [TestMethod]
    public async Task FileStore_NullJsonReturnsEmptyAndMalformedJsonThrowsJsonException()
    {
        FileAtlasOpsStore store = new(this.temporaryRoot);
        string path = Path.Combine(this.temporaryRoot, "AtlasOpsProject.json");
        await File.WriteAllTextAsync(path, "null");

        Assert.HasCount(0, await store.LoadAsync<AtlasOpsProject>());

        await File.WriteAllTextAsync(
            path,
            "{ definitely-not-json");

        await Assert.ThrowsExactlyAsync<JsonException>(() => store.LoadAsync<AtlasOpsProject>());
    }

    [TestMethod]
    public void FileStore_InvalidDirectoryPath_ThrowsIOException()
    {
        Directory.CreateDirectory(this.temporaryRoot);
        string parentFile = Path.Combine(this.temporaryRoot, "regular-file");
        File.WriteAllText(parentFile, "content");

        try
        {
            _ = new FileAtlasOpsStore(Path.Combine(parentFile, "child"));
            Assert.Fail("Expected an IOException for a directory nested beneath a regular file.");
        }
        catch (IOException)
        {
            // The exact IOException subtype varies by file system; the deterministic contract is I/O failure.
        }
    }

    [TestMethod]
    [DataRow("libsql://atlasops.example")]
    [DataRow("LIBSQL://atlasops.example")]
    [DataRow("https://atlasops.example/database")]
    public void TursoStore_ValidLibsqlAndHttpsUrls_ConstructOffline(string url)
    {
        TursoAtlasOpsStore store = new(url, "offline-token");

        Assert.AreEqual("Turso/libSQL", store.Kind);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("not-a-url")]
    [DataRow("/relative")]
    [DataRow("http://atlasops.example")]
    [DataRow("ftp://atlasops.example")]
    public void TursoStore_InvalidUrls_ThrowArgumentException(string url)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => new TursoAtlasOpsStore(url, "offline-token"));

        Assert.AreEqual("url", exception.ParamName);
        StringAssert.Contains(exception.Message, "libsql:// or https://");
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void TursoStore_BlankToken_ThrowsArgumentException(string token)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => new TursoAtlasOpsStore("https://atlasops.example", token));

        Assert.AreEqual("token", exception.ParamName);
        StringAssert.Contains(exception.Message, "authentication token");
    }

    [TestMethod]
    public void StoreFactory_SelectsConfiguredProvider()
    {
        AtlasOpsSettings localSettings = new()
        {
            LocalDataPath = Path.Combine(this.temporaryRoot, "local"),
            UseTurso = false,
            TursoUrl = "invalid-is-ignored",
            TursoToken = string.Empty,
        };
        AtlasOpsSettings tursoSettings = new()
        {
            LocalDataPath = Path.Combine(this.temporaryRoot, "unused"),
            UseTurso = true,
            TursoUrl = "libsql://atlasops.example",
            TursoToken = "offline-token",
        };

        IAtlasOpsStore local = AtlasOpsStoreFactory.Create(localSettings);
        IAtlasOpsStore turso = AtlasOpsStoreFactory.Create(tursoSettings);

        Assert.IsInstanceOfType<FileAtlasOpsStore>(local);
        Assert.AreEqual("Local JSON", local.Kind);
        Assert.IsTrue(Directory.Exists(localSettings.LocalDataPath));
        Assert.IsInstanceOfType<TursoAtlasOpsStore>(turso);
        Assert.AreEqual("Turso/libSQL", turso.Kind);
        Assert.IsFalse(Directory.Exists(tursoSettings.LocalDataPath));
    }
}