namespace AtlasOps.Features.Compute.ComputePatchGovernance;

using AtlasOps.Features;

public sealed class ComputePatchGovernanceService(
    IAtlasOpsCapabilityRepository<ComputePatchGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputePatchGovernanceValidator validator = new();
    private readonly ComputePatchGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputePatchGovernanceChanged>> ExecuteAsync(
        UpdateComputePatchGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputePatchGovernanceChanged>.Invalid(issues);
        }

        ComputePatchGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputePatchGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputePatchGovernanceChanged>.Invalid(
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

        ComputePatchGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputePatchGovernanceChanged>.Success(changed);
    }
}