namespace AtlasOps.Features.Compute.ComputeTemplateGovernance;

using AtlasOps.Features;

public sealed class ComputeTemplateGovernanceService(
    IAtlasOpsCapabilityRepository<ComputeTemplateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeTemplateGovernanceValidator validator = new();
    private readonly ComputeTemplateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeTemplateGovernanceChanged>> ExecuteAsync(
        UpdateComputeTemplateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeTemplateGovernanceChanged>.Invalid(issues);
        }

        ComputeTemplateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeTemplateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeTemplateGovernanceChanged>.Invalid(
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

        ComputeTemplateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeTemplateGovernanceChanged>.Success(changed);
    }
}