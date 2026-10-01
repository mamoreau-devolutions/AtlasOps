namespace AtlasOps.Features.Observability.LogSourceGovernance;

using AtlasOps.Features;

public sealed class LogSourceGovernanceService(
    IAtlasOpsCapabilityRepository<LogSourceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogSourceGovernanceValidator validator = new();
    private readonly LogSourceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogSourceGovernanceChanged>> ExecuteAsync(
        UpdateLogSourceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogSourceGovernanceChanged>.Invalid(issues);
        }

        LogSourceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogSourceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogSourceGovernanceChanged>.Invalid(
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

        LogSourceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogSourceGovernanceChanged>.Success(changed);
    }
}