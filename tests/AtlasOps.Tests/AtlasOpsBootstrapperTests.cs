namespace AtlasOps.Tests;

using AtlasOps.App.Services;
using AtlasOps.Core;

[TestClass]
public sealed class AtlasOpsBootstrapperTests
{
    private string temporaryRoot = null!;

    [TestInitialize]
    public void Initialize()
    {
        this.temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            "AtlasOps.Bootstrapper.Tests",
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
    public async Task LoadWorkspaceAsync_EmptyStore_SeedsEveryGeneratedModelType()
    {
        AtlasOpsBootstrapper bootstrapper = new(this.temporaryRoot, forceLocal: true);

        AtlasOpsGeneratedWorkspace workspace = await bootstrapper.LoadWorkspaceAsync();

        Assert.HasCount(29, workspace.Entities);
        CollectionAssert.AreEquivalent(
            ModelTestData.Cases.Select(static item => item.Type).ToArray(),
            workspace.Entities.Select(static entity => entity.GetType()).Distinct().ToArray());
        Assert.AreEqual(
            "AtlasOps launch",
            workspace.Entities.OfType<AtlasOpsProject>().Single(project => project.Priority == 1).Name);
        Assert.AreEqual(
            "Turso production",
            workspace.Entities.OfType<AtlasOpsConnection>().Single(connection => connection.Protocol == "libSQL").Name);
        Assert.AreEqual(
            70,
            workspace.Entities.OfType<AtlasOpsTaskItem>().Single(task => task.IsHighPriority).Completion);
        Assert.AreEqual(
            "Production operator",
            workspace.Entities.OfType<AtlasOpsCredential>().Single().Name);
        Assert.AreEqual(
            "Platform",
            workspace.Entities.OfType<AtlasOpsTeam>().Single().Name);

        foreach (ModelCase modelCase in ModelTestData.Cases)
        {
            Assert.IsTrue(
                workspace.Entities.Any(entity => entity.GetType() == modelCase.Type),
                $"Seed is missing {modelCase.Type.Name}.");
            Assert.IsTrue(
                File.Exists(Path.Combine(this.temporaryRoot, $"{modelCase.Type.Name}.json")),
                $"Seed was not saved for {modelCase.Type.Name}.");
        }
    }

    [TestMethod]
    public async Task SaveThenLoadWorkspaceAsync_ReloadsPersistedChangesWithoutReseeding()
    {
        AtlasOpsBootstrapper firstBootstrapper = new(this.temporaryRoot, forceLocal: true);
        AtlasOpsGeneratedWorkspace seeded = await firstBootstrapper.LoadWorkspaceAsync();
        AtlasOpsProject project = seeded.Entities.OfType<AtlasOpsProject>().First();
        string persistedId = project.Id;
        project.Name = "Persisted project name";

        await firstBootstrapper.SaveWorkspaceAsync(seeded);
        AtlasOpsBootstrapper secondBootstrapper = new(this.temporaryRoot, forceLocal: true);
        AtlasOpsGeneratedWorkspace reloaded = await secondBootstrapper.LoadWorkspaceAsync();

        Assert.HasCount(29, reloaded.Entities);
        AtlasOpsProject persisted = reloaded.Entities
            .OfType<AtlasOpsProject>()
            .Single(item => item.Id == persistedId);
        Assert.AreEqual("Persisted project name", persisted.Name);
        CollectionAssert.AreEquivalent(
            ModelTestData.Cases.Select(static item => item.Type).ToArray(),
            reloaded.Entities.Select(static entity => entity.GetType()).Distinct().ToArray());
    }

    [TestMethod]
    public void Constructor_ForcedLocal_UsesProvidedSettingsWithoutEnvironmentNetwork()
    {
        AtlasOpsBootstrapper bootstrapper = new(this.temporaryRoot, forceLocal: true);

        Assert.AreEqual("AtlasOps Operations Workspace", bootstrapper.Settings.WorkspaceName);
        Assert.AreEqual(this.temporaryRoot, bootstrapper.Settings.LocalDataPath);
        Assert.IsFalse(bootstrapper.Settings.UseTurso);
        Assert.AreEqual("Local JSON", bootstrapper.PersistenceKind);
        Assert.IsTrue(Directory.Exists(this.temporaryRoot));
    }
}