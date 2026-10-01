namespace AtlasOps.Features.Database.DatabaseQueryGovernance;

using AtlasOps.Features;

public sealed class DatabaseQueryGovernanceService(
    IAtlasOpsCapabilityRepository<DatabaseQueryGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseQueryGovernanceValidator validator = new();
    private readonly DatabaseQueryGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseQueryGovernanceChanged>> ExecuteAsync(
        UpdateDatabaseQueryGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseQueryGovernanceChanged>.Invalid(issues);
        }

        DatabaseQueryGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseQueryGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseQueryGovernanceChanged>.Invalid(
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

        DatabaseQueryGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseQueryGovernanceChanged>.Success(changed);
    }
}