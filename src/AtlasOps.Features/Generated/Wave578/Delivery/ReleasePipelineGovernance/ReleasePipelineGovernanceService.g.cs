namespace AtlasOps.Features.Delivery.ReleasePipelineGovernance;

using AtlasOps.Features;

public sealed class ReleasePipelineGovernanceService(
    IAtlasOpsCapabilityRepository<ReleasePipelineGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleasePipelineGovernanceValidator validator = new();
    private readonly ReleasePipelineGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleasePipelineGovernanceChanged>> ExecuteAsync(
        UpdateReleasePipelineGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleasePipelineGovernanceChanged>.Invalid(issues);
        }

        ReleasePipelineGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleasePipelineGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleasePipelineGovernanceChanged>.Invalid(
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

        ReleasePipelineGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleasePipelineGovernanceChanged>.Success(changed);
    }
}