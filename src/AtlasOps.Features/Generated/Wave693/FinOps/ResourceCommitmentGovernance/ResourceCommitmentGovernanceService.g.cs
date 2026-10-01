namespace AtlasOps.Features.FinOps.ResourceCommitmentGovernance;

using AtlasOps.Features;

public sealed class ResourceCommitmentGovernanceService(
    IAtlasOpsCapabilityRepository<ResourceCommitmentGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResourceCommitmentGovernanceValidator validator = new();
    private readonly ResourceCommitmentGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResourceCommitmentGovernanceChanged>> ExecuteAsync(
        UpdateResourceCommitmentGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResourceCommitmentGovernanceChanged>.Invalid(issues);
        }

        ResourceCommitmentGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResourceCommitmentGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResourceCommitmentGovernanceChanged>.Invalid(
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

        ResourceCommitmentGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResourceCommitmentGovernanceChanged>.Success(changed);
    }
}