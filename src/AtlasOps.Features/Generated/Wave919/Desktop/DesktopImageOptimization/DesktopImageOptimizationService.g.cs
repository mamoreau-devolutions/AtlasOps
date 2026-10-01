namespace AtlasOps.Features.Desktop.DesktopImageOptimization;

using AtlasOps.Features;

public sealed class DesktopImageOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopImageOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopImageOptimizationValidator validator = new();
    private readonly DesktopImageOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopImageOptimizationChanged>> ExecuteAsync(
        UpdateDesktopImageOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopImageOptimizationChanged>.Invalid(issues);
        }

        DesktopImageOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopImageOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopImageOptimizationChanged>.Invalid(
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

        DesktopImageOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopImageOptimizationChanged>.Success(changed);
    }
}