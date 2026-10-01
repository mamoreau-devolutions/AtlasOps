namespace AtlasOps.Features.Compute.ComputeConsoleGovernance;

using AtlasOps.Features;

public sealed class ComputeConsoleGovernanceService(
    IAtlasOpsCapabilityRepository<ComputeConsoleGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeConsoleGovernanceValidator validator = new();
    private readonly ComputeConsoleGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeConsoleGovernanceChanged>> ExecuteAsync(
        UpdateComputeConsoleGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeConsoleGovernanceChanged>.Invalid(issues);
        }

        ComputeConsoleGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeConsoleGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeConsoleGovernanceChanged>.Invalid(
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

        ComputeConsoleGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeConsoleGovernanceChanged>.Success(changed);
    }
}