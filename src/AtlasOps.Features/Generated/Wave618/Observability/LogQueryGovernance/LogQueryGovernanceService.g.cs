namespace AtlasOps.Features.Observability.LogQueryGovernance;

using AtlasOps.Features;

public sealed class LogQueryGovernanceService(
    IAtlasOpsCapabilityRepository<LogQueryGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly LogQueryGovernanceValidator validator = new();
    private readonly LogQueryGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<LogQueryGovernanceChanged>> ExecuteAsync(
        UpdateLogQueryGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LogQueryGovernanceChanged>.Invalid(issues);
        }

        LogQueryGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LogQueryGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LogQueryGovernanceChanged>.Invalid(
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

        LogQueryGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LogQueryGovernanceChanged>.Success(changed);
    }
}