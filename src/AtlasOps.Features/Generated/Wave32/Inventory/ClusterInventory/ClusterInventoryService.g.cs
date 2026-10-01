namespace AtlasOps.Features.Inventory.ClusterInventory;

using AtlasOps.Features;

public sealed class ClusterInventoryService(
    IAtlasOpsCapabilityRepository<ClusterInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ClusterInventoryValidator validator = new();
    private readonly ClusterInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ClusterInventoryChanged>> ExecuteAsync(
        UpdateClusterInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ClusterInventoryChanged>.Invalid(issues);
        }

        ClusterInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ClusterInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ClusterInventoryChanged>.Invalid(
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

        ClusterInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ClusterInventoryChanged>.Success(changed);
    }
}