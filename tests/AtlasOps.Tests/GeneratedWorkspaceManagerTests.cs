namespace AtlasOps.Tests;

using AtlasOps.Core;
using AtlasOps.Core.Generated;

[TestClass]
public sealed class GeneratedWorkspaceManagerTests
{
    [TestMethod]
    public async Task LoadAsync_LoadsAndAggregatesEveryGeneratedModelType()
    {
        RecordingStore store = new();
        foreach (ModelCase modelCase in ModelTestData.Cases)
        {
            store.LoadedEntities[modelCase.Type] = [ModelTestData.CreatePopulated(modelCase)];
        }

        AtlasOpsGeneratedWorkspace workspace =
            await AtlasOpsGeneratedWorkspaceManager.LoadAsync(store);

        Assert.HasCount(20, store.LoadCalls);
        Assert.HasCount(20, workspace.Entities);
        CollectionAssert.AreEquivalent(
            ModelTestData.Cases.Select(static item => item.Type).ToArray(),
            store.LoadCalls.Select(static item => item.ModelType).ToArray());
        CollectionAssert.AreEquivalent(
            ModelTestData.Cases.Select(static item => item.Type).ToArray(),
            workspace.Entities.Select(static item => item.GetType()).ToArray());
        foreach (IAtlasOpsEntity entity in workspace.Entities)
        {
            Assert.AreEqual($"id-{entity.GetType().Name}", entity.Id);
        }
    }

    [TestMethod]
    public async Task SaveAsync_PartitionsAndSavesEveryGeneratedModelType()
    {
        RecordingStore store = new();
        List<IAtlasOpsEntity> entities = ModelTestData.Cases
            .Select(ModelTestData.CreatePopulated)
            .Append(new UnsupportedEntity())
            .ToList();

        await AtlasOpsGeneratedWorkspaceManager.SaveAsync(store, entities);

        Assert.HasCount(20, store.SaveCalls);
        CollectionAssert.AreEquivalent(
            ModelTestData.Cases.Select(static item => item.Type).ToArray(),
            store.SaveCalls.Select(static item => item.ModelType).ToArray());
        foreach (StoreCall call in store.SaveCalls)
        {
            IAtlasOpsEntity saved = call.Entities.Single();
            Assert.AreEqual(call.ModelType, saved.GetType());
            Assert.AreEqual($"id-{call.ModelType.Name}", saved.Id);
        }
        Assert.IsFalse(store.SaveCalls.SelectMany(static call => call.Entities).OfType<UnsupportedEntity>().Any());
    }

    [TestMethod]
    public async Task LoadAndSave_ForwardCancellationTokenToEveryModelOperation()
    {
        using CancellationTokenSource source = new();
        RecordingStore store = new();

        await AtlasOpsGeneratedWorkspaceManager.LoadAsync(store, source.Token);
        await AtlasOpsGeneratedWorkspaceManager.SaveAsync(store, [], source.Token);

        Assert.HasCount(20, store.LoadCalls);
        Assert.HasCount(20, store.SaveCalls);
        Assert.IsTrue(store.LoadCalls.All(call => call.Token == source.Token));
        Assert.IsTrue(store.SaveCalls.All(call => call.Token == source.Token));

        source.Cancel();
        RecordingStore cancelingStore = new() { ThrowOnCancellation = true };
        await Assert.ThrowsExactlyAsync<OperationCanceledException>(
            () => AtlasOpsGeneratedWorkspaceManager.LoadAsync(cancelingStore, source.Token));
        Assert.HasCount(1, cancelingStore.LoadCalls);
        Assert.AreEqual(source.Token, cancelingStore.LoadCalls[0].Token);
    }

    private sealed class RecordingStore : IAtlasOpsStore
    {
        internal Dictionary<Type, IReadOnlyList<IAtlasOpsEntity>> LoadedEntities { get; } = [];

        internal List<StoreCall> LoadCalls { get; } = [];

        internal List<StoreCall> SaveCalls { get; } = [];

        internal bool ThrowOnCancellation { get; init; }

        public string Kind => "Recording";

        public Task<IReadOnlyList<TModel>> LoadAsync<TModel>(
            CancellationToken cancellationToken = default)
            where TModel : class, IAtlasOpsEntity
        {
            this.LoadCalls.Add(new StoreCall(typeof(TModel), [], cancellationToken));
            if (this.ThrowOnCancellation)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            IReadOnlyList<TModel> result = this.LoadedEntities.TryGetValue(
                typeof(TModel),
                out IReadOnlyList<IAtlasOpsEntity>? entities)
                ? entities.Cast<TModel>().ToArray()
                : [];
            return Task.FromResult(result);
        }

        public Task SaveAsync<TModel>(
            IEnumerable<TModel> models,
            CancellationToken cancellationToken = default)
            where TModel : class, IAtlasOpsEntity
        {
            this.SaveCalls.Add(new StoreCall(
                typeof(TModel),
                models.Cast<IAtlasOpsEntity>().ToArray(),
                cancellationToken));
            if (this.ThrowOnCancellation)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            return Task.CompletedTask;
        }
    }

    private sealed record StoreCall(
        Type ModelType,
        IReadOnlyList<IAtlasOpsEntity> Entities,
        CancellationToken Token);
}