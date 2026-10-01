namespace AtlasOps.Features.Database.DatabaseMaintenanceGovernance;

using AtlasOps.Features;

public sealed class DatabaseMaintenanceGovernanceService(
    IAtlasOpsCapabilityRepository<DatabaseMaintenanceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseMaintenanceGovernanceValidator validator = new();
    private readonly DatabaseMaintenanceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseMaintenanceGovernanceChanged>> ExecuteAsync(
        UpdateDatabaseMaintenanceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseMaintenanceGovernanceChanged>.Invalid(issues);
        }

        DatabaseMaintenanceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseMaintenanceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseMaintenanceGovernanceChanged>.Invalid(
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

        DatabaseMaintenanceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseMaintenanceGovernanceChanged>.Success(changed);
    }
}