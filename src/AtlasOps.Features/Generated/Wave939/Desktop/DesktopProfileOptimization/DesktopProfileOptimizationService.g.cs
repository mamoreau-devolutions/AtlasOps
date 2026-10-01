namespace AtlasOps.Features.Desktop.DesktopProfileOptimization;

using AtlasOps.Features;

public sealed class DesktopProfileOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopProfileOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopProfileOptimizationValidator validator = new();
    private readonly DesktopProfileOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopProfileOptimizationChanged>> ExecuteAsync(
        UpdateDesktopProfileOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopProfileOptimizationChanged>.Invalid(issues);
        }

        DesktopProfileOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopProfileOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopProfileOptimizationChanged>.Invalid(
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

        DesktopProfileOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopProfileOptimizationChanged>.Success(changed);
    }
}