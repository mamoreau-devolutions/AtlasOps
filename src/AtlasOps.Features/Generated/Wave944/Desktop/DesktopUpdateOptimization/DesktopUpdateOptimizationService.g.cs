namespace AtlasOps.Features.Desktop.DesktopUpdateOptimization;

using AtlasOps.Features;

public sealed class DesktopUpdateOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopUpdateOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopUpdateOptimizationValidator validator = new();
    private readonly DesktopUpdateOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopUpdateOptimizationChanged>> ExecuteAsync(
        UpdateDesktopUpdateOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopUpdateOptimizationChanged>.Invalid(issues);
        }

        DesktopUpdateOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopUpdateOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopUpdateOptimizationChanged>.Invalid(
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

        DesktopUpdateOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopUpdateOptimizationChanged>.Success(changed);
    }
}