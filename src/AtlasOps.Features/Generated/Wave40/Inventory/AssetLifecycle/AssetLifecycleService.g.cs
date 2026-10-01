namespace AtlasOps.Features.Inventory.AssetLifecycle;

using AtlasOps.Features;

public sealed class AssetLifecycleService(
    IAtlasOpsCapabilityRepository<AssetLifecycleItem> repository,
    TimeProvider timeProvider)
{
    private readonly AssetLifecycleValidator validator = new();
    private readonly AssetLifecyclePolicy policy = new();

    public async Task<AtlasOpsOperationResult<AssetLifecycleChanged>> ExecuteAsync(
        UpdateAssetLifecycleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AssetLifecycleChanged>.Invalid(issues);
        }

        AssetLifecycleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AssetLifecycleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AssetLifecycleChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        AssetLifecycleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AssetLifecycleChanged>.Success(changed);
    }
}