namespace AtlasOps.Features.Database.DatabaseSchemaGovernance;

using AtlasOps.Features;

public sealed class DatabaseSchemaGovernanceService(
    IAtlasOpsCapabilityRepository<DatabaseSchemaGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseSchemaGovernanceValidator validator = new();
    private readonly DatabaseSchemaGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseSchemaGovernanceChanged>> ExecuteAsync(
        UpdateDatabaseSchemaGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseSchemaGovernanceChanged>.Invalid(issues);
        }

        DatabaseSchemaGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseSchemaGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseSchemaGovernanceChanged>.Invalid(
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

        DatabaseSchemaGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseSchemaGovernanceChanged>.Success(changed);
    }
}