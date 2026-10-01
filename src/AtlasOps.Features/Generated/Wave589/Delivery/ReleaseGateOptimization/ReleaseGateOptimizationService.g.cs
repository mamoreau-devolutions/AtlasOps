namespace AtlasOps.Features.Delivery.ReleaseGateOptimization;

using AtlasOps.Features;

public sealed class ReleaseGateOptimizationService(
    IAtlasOpsCapabilityRepository<ReleaseGateOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseGateOptimizationValidator validator = new();
    private readonly ReleaseGateOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseGateOptimizationChanged>> ExecuteAsync(
        UpdateReleaseGateOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseGateOptimizationChanged>.Invalid(issues);
        }

        ReleaseGateOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseGateOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseGateOptimizationChanged>.Invalid(
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

        ReleaseGateOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseGateOptimizationChanged>.Success(changed);
    }
}