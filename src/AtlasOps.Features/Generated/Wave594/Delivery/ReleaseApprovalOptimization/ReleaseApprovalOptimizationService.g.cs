namespace AtlasOps.Features.Delivery.ReleaseApprovalOptimization;

using AtlasOps.Features;

public sealed class ReleaseApprovalOptimizationService(
    IAtlasOpsCapabilityRepository<ReleaseApprovalOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseApprovalOptimizationValidator validator = new();
    private readonly ReleaseApprovalOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseApprovalOptimizationChanged>> ExecuteAsync(
        UpdateReleaseApprovalOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseApprovalOptimizationChanged>.Invalid(issues);
        }

        ReleaseApprovalOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseApprovalOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseApprovalOptimizationChanged>.Invalid(
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

        ReleaseApprovalOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseApprovalOptimizationChanged>.Success(changed);
    }
}