namespace AtlasOps.Operations.Runtime;

using AtlasOps.Operations.Contracts;

public sealed class InMemoryCheckpointStore : ICheckpointStore
{
    private readonly object gate = new();
    private readonly Dictionary<string, SynchronizationCheckpoint> checkpoints =
        new(StringComparer.Ordinal);

    public ValueTask<SynchronizationCheckpoint?> GetAsync(
        string providerId,
        string scope,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string key = CreateKey(providerId, scope);

        lock (this.gate)
        {
            this.checkpoints.TryGetValue(key, out SynchronizationCheckpoint? checkpoint);
            return ValueTask.FromResult(checkpoint);
        }
    }

    public ValueTask<CheckpointMutationResult> WriteAsync(
        SynchronizationCheckpoint checkpoint,
        long expectedRevision,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(checkpoint.ProviderId) ||
            string.IsNullOrWhiteSpace(checkpoint.Scope) ||
            string.IsNullOrWhiteSpace(checkpoint.Cursor))
        {
            return ValueTask.FromResult(
                new CheckpointMutationResult(false, "Provider, scope, and cursor are required.", null));
        }

        string key = CreateKey(checkpoint.ProviderId, checkpoint.Scope);
        lock (this.gate)
        {
            long currentRevision = this.checkpoints.TryGetValue(
                key,
                out SynchronizationCheckpoint? current)
                ? current.Revision
                : 0;

            if (currentRevision != expectedRevision)
            {
                return ValueTask.FromResult(
                    new CheckpointMutationResult(false, "The checkpoint revision is stale.", current));
            }

            if (checkpoint.Revision != expectedRevision + 1)
            {
                return ValueTask.FromResult(
                    new CheckpointMutationResult(false, "The next checkpoint revision must be monotonic.", current));
            }

            this.checkpoints[key] = checkpoint;
            return ValueTask.FromResult(
                new CheckpointMutationResult(true, string.Empty, checkpoint));
        }
    }

    private static string CreateKey(string providerId, string scope)
    {
        return $"{providerId.Trim().ToUpperInvariant()}:{scope.Trim().ToUpperInvariant()}";
    }
}
