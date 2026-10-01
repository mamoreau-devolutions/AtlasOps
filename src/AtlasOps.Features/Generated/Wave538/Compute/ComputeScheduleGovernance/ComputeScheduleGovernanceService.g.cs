namespace AtlasOps.Features.Compute.ComputeScheduleGovernance;

using AtlasOps.Features;

public sealed class ComputeScheduleGovernanceService(
    IAtlasOpsCapabilityRepository<ComputeScheduleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScheduleGovernanceValidator validator = new();
    private readonly ComputeScheduleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScheduleGovernanceChanged>> ExecuteAsync(
        UpdateComputeScheduleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScheduleGovernanceChanged>.Invalid(issues);
        }

        ComputeScheduleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScheduleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScheduleGovernanceChanged>.Invalid(
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

        ComputeScheduleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScheduleGovernanceChanged>.Success(changed);
    }
}