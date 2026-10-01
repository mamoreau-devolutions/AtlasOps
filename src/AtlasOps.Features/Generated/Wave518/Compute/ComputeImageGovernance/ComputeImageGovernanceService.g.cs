namespace AtlasOps.Features.Compute.ComputeImageGovernance;

using AtlasOps.Features;

public sealed class ComputeImageGovernanceService(
    IAtlasOpsCapabilityRepository<ComputeImageGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeImageGovernanceValidator validator = new();
    private readonly ComputeImageGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeImageGovernanceChanged>> ExecuteAsync(
        UpdateComputeImageGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeImageGovernanceChanged>.Invalid(issues);
        }

        ComputeImageGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeImageGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeImageGovernanceChanged>.Invalid(
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

        ComputeImageGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeImageGovernanceChanged>.Success(changed);
    }
}