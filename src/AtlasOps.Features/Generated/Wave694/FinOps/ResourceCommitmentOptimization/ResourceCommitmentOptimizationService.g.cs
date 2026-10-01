namespace AtlasOps.Features.FinOps.ResourceCommitmentOptimization;

using AtlasOps.Features;

public sealed class ResourceCommitmentOptimizationService(
    IAtlasOpsCapabilityRepository<ResourceCommitmentOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ResourceCommitmentOptimizationValidator validator = new();
    private readonly ResourceCommitmentOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ResourceCommitmentOptimizationChanged>> ExecuteAsync(
        UpdateResourceCommitmentOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ResourceCommitmentOptimizationChanged>.Invalid(issues);
        }

        ResourceCommitmentOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ResourceCommitmentOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ResourceCommitmentOptimizationChanged>.Invalid(
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

        ResourceCommitmentOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ResourceCommitmentOptimizationChanged>.Success(changed);
    }
}