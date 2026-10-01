namespace AtlasOps.Features.Desktop.DesktopPeripheralOptimization;

using AtlasOps.Features;

public sealed class DesktopPeripheralOptimizationService(
    IAtlasOpsCapabilityRepository<DesktopPeripheralOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly DesktopPeripheralOptimizationValidator validator = new();
    private readonly DesktopPeripheralOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DesktopPeripheralOptimizationChanged>> ExecuteAsync(
        UpdateDesktopPeripheralOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DesktopPeripheralOptimizationChanged>.Invalid(issues);
        }

        DesktopPeripheralOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DesktopPeripheralOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DesktopPeripheralOptimizationChanged>.Invalid(
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

        DesktopPeripheralOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DesktopPeripheralOptimizationChanged>.Success(changed);
    }
}