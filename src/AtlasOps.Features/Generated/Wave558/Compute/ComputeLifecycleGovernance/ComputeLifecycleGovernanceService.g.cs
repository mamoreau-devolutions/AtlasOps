namespace AtlasOps.Features.Compute.ComputeLifecycleGovernance;

using AtlasOps.Features;

public sealed class ComputeLifecycleGovernanceService(
    IAtlasOpsCapabilityRepository<ComputeLifecycleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeLifecycleGovernanceValidator validator = new();
    private readonly ComputeLifecycleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeLifecycleGovernanceChanged>> ExecuteAsync(
        UpdateComputeLifecycleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeLifecycleGovernanceChanged>.Invalid(issues);
        }

        ComputeLifecycleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeLifecycleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeLifecycleGovernanceChanged>.Invalid(
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

        ComputeLifecycleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeLifecycleGovernanceChanged>.Success(changed);
    }
}