namespace AtlasOps.Features.Inventory.HostInventory;

using AtlasOps.Features;

public sealed class HostInventoryService(
    IAtlasOpsCapabilityRepository<HostInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly HostInventoryValidator validator = new();
    private readonly HostInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<HostInventoryChanged>> ExecuteAsync(
        UpdateHostInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<HostInventoryChanged>.Invalid(issues);
        }

        HostInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new HostInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<HostInventoryChanged>.Invalid(
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

        HostInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<HostInventoryChanged>.Success(changed);
    }
}