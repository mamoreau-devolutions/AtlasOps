namespace AtlasOps.Features.Inventory.LicenseInventory;

using AtlasOps.Features;

public sealed class LicenseInventoryService(
    IAtlasOpsCapabilityRepository<LicenseInventoryItem> repository,
    TimeProvider timeProvider)
{
    private readonly LicenseInventoryValidator validator = new();
    private readonly LicenseInventoryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LicenseInventoryChanged>> ExecuteAsync(
        UpdateLicenseInventoryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LicenseInventoryChanged>.Invalid(issues);
        }

        LicenseInventoryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LicenseInventoryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LicenseInventoryChanged>.Invalid(
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

        LicenseInventoryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LicenseInventoryChanged>.Success(changed);
    }
}