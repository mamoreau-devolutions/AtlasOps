namespace AtlasOps.Features.Delivery.ReleaseCalendarOptimization;

using AtlasOps.Features;

public sealed class ReleaseCalendarOptimizationService(
    IAtlasOpsCapabilityRepository<ReleaseCalendarOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseCalendarOptimizationValidator validator = new();
    private readonly ReleaseCalendarOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseCalendarOptimizationChanged>> ExecuteAsync(
        UpdateReleaseCalendarOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseCalendarOptimizationChanged>.Invalid(issues);
        }

        ReleaseCalendarOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseCalendarOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseCalendarOptimizationChanged>.Invalid(
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

        ReleaseCalendarOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseCalendarOptimizationChanged>.Success(changed);
    }
}