namespace AtlasOps.Features.Database.DatabaseIndexGovernance;

using AtlasOps.Features;

public sealed class DatabaseIndexGovernanceService(
    IAtlasOpsCapabilityRepository<DatabaseIndexGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly DatabaseIndexGovernanceValidator validator = new();
    private readonly DatabaseIndexGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<DatabaseIndexGovernanceChanged>> ExecuteAsync(
        UpdateDatabaseIndexGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DatabaseIndexGovernanceChanged>.Invalid(issues);
        }

        DatabaseIndexGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DatabaseIndexGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DatabaseIndexGovernanceChanged>.Invalid(
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

        DatabaseIndexGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DatabaseIndexGovernanceChanged>.Success(changed);
    }
}