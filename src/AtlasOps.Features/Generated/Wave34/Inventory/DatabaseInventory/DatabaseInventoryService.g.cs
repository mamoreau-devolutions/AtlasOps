namespace AtlasOps.Features.Inventory.DatabaseInventory;

using AtlasOps.Features;

public sealed class DatabaseInventoryService(
    IAtlasOpsCapabilityRepository<DatabaseInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseInventoryValidator validator = new();
    private readonly DatabaseInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseInventoryChanged>> ExecuteAsync(
        UpdateDatabaseInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseInventoryChanged>.Invalid(issues);
        }

        DatabaseInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseInventoryChanged>.Invalid(
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

        DatabaseInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseInventoryChanged>.Success(changed);
    }
}