namespace AtlasOps.Features.Desktop.DesktopPoolOptimization;

using AtlasOps.Features;

public sealed class DesktopPoolOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopPoolOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPoolOptimizationValidator validator = new();
    private readonly DesktopPoolOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPoolOptimizationChanged>> ExecuteAsync(
        UpdateDesktopPoolOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPoolOptimizationChanged>.Invalid(issues);
        }

        DesktopPoolOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPoolOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPoolOptimizationChanged>.Invalid(
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

        DesktopPoolOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPoolOptimizationChanged>.Success(changed);
    }
}