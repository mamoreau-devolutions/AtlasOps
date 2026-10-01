namespace AtlasOps.Features.Database.DatabaseCredentialGovernance;

using AtlasOps.Features;

public sealed class DatabaseCredentialGovernanceService(
    IAtlasOpsCapabilityRepository<DatabaseCredentialGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseCredentialGovernanceValidator validator = new();
    private readonly DatabaseCredentialGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseCredentialGovernanceChanged>> ExecuteAsync(
        UpdateDatabaseCredentialGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseCredentialGovernanceChanged>.Invalid(issues);
        }

        DatabaseCredentialGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseCredentialGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseCredentialGovernanceChanged>.Invalid(
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

        DatabaseCredentialGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseCredentialGovernanceChanged>.Success(changed);
    }
}