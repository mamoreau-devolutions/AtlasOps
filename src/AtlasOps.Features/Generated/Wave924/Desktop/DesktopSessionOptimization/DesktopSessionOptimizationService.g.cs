namespace AtlasOps.Features.Desktop.DesktopSessionOptimization;

using AtlasOps.Features;

public sealed class DesktopSessionOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopSessionOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopSessionOptimizationValidator validator = new();
    private readonly DesktopSessionOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopSessionOptimizationChanged>> ExecuteAsync(
        UpdateDesktopSessionOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopSessionOptimizationChanged>.Invalid(issues);
        }

        DesktopSessionOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopSessionOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopSessionOptimizationChanged>.Invalid(
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

        DesktopSessionOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopSessionOptimizationChanged>.Success(changed);
    }
}