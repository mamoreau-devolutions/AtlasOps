namespace AtlasOps.Features.Inventory.NetworkInventory;

using AtlasOps.Features;

public sealed class NetworkInventoryService(
    IAtlasOpsCapabilityRepository<NetworkInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkInventoryValidator validator = new();
    private readonly NetworkInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkInventoryChanged>> ExecuteAsync(
        UpdateNetworkInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkInventoryChanged>.Invalid(issues);
        }

        NetworkInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkInventoryChanged>.Invalid(
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

        NetworkInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkInventoryChanged>.Success(changed);
    }
}