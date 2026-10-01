namespace AtlasOps.Features.Inventory.SiteInventory;

using AtlasOps.Features;

public sealed class SiteInventoryService(
    IAtlasOpsCapabilityRepository<SiteInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SiteInventoryValidator validator = new();
    private readonly SiteInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SiteInventoryChanged>> ExecuteAsync(
        UpdateSiteInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SiteInventoryChanged>.Invalid(issues);
        }

        SiteInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SiteInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SiteInventoryChanged>.Invalid(
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

        SiteInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SiteInventoryChanged>.Success(changed);
    }
}