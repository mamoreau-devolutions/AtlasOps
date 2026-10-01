namespace AtlasOps.Features.Desktop.DesktopApplicationOptimization;

using AtlasOps.Features;

public sealed class DesktopApplicationOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopApplicationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopApplicationOptimizationValidator validator = new();
    private readonly DesktopApplicationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopApplicationOptimizationChanged>> ExecuteAsync(
        UpdateDesktopApplicationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopApplicationOptimizationChanged>.Invalid(issues);
        }

        DesktopApplicationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopApplicationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopApplicationOptimizationChanged>.Invalid(
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

        DesktopApplicationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopApplicationOptimizationChanged>.Success(changed);
    }
}