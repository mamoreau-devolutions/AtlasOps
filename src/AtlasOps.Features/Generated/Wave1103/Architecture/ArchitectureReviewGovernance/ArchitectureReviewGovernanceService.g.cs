namespace AtlasOps.Features.Architecture.ArchitectureReviewGovernance;

using AtlasOps.Features;

public sealed class ArchitectureReviewGovernanceService(
    IAtlasOpsCapabilityRepository<ArchitectureReviewGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureReviewGovernanceValidator validator = new();
    private readonly ArchitectureReviewGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureReviewGovernanceChanged>> ExecuteAsync(
        UpdateArchitectureReviewGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureReviewGovernanceChanged>.Invalid(issues);
        }

        ArchitectureReviewGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureReviewGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureReviewGovernanceChanged>.Invalid(
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

        ArchitectureReviewGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureReviewGovernanceChanged>.Success(changed);
    }
}