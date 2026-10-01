namespace AtlasOps.Features.Desktop.DesktopHealthOptimization;

using AtlasOps.Features;

public sealed class DesktopHealthOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopHealthOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopHealthOptimizationValidator validator = new();
    private readonly DesktopHealthOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopHealthOptimizationChanged>> ExecuteAsync(
        UpdateDesktopHealthOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopHealthOptimizationChanged>.Invalid(issues);
        }

        DesktopHealthOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopHealthOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopHealthOptimizationChanged>.Invalid(
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

        DesktopHealthOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopHealthOptimizationChanged>.Success(changed);
    }
}