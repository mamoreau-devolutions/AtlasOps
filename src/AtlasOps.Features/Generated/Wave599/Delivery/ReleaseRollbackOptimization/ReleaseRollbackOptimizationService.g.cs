namespace AtlasOps.Features.Delivery.ReleaseRollbackOptimization;

using AtlasOps.Features;

public sealed class ReleaseRollbackOptimizationService(
    IAtlasOpsCapabilityRepository<ReleaseRollbackOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseRollbackOptimizationValidator validator = new();
    private readonly ReleaseRollbackOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseRollbackOptimizationChanged>> ExecuteAsync(
        UpdateReleaseRollbackOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseRollbackOptimizationChanged>.Invalid(issues);
        }

        ReleaseRollbackOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseRollbackOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseRollbackOptimizationChanged>.Invalid(
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

        ReleaseRollbackOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseRollbackOptimizationChanged>.Success(changed);
    }
}