namespace AtlasOps.Features.Database.DatabaseRestoreGovernance;

using AtlasOps.Features;

public sealed class DatabaseRestoreGovernanceService(
    IAtlasOpsCapabilityRepository<DatabaseRestoreGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseRestoreGovernanceValidator validator = new();
    private readonly DatabaseRestoreGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseRestoreGovernanceChanged>> ExecuteAsync(
        UpdateDatabaseRestoreGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseRestoreGovernanceChanged>.Invalid(issues);
        }

        DatabaseRestoreGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseRestoreGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseRestoreGovernanceChanged>.Invalid(
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

        DatabaseRestoreGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseRestoreGovernanceChanged>.Success(changed);
    }
}