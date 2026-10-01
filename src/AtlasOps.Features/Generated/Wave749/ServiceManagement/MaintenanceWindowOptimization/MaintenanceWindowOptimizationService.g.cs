namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowOptimization;

using AtlasOps.Features;

public sealed class MaintenanceWindowOptimizationService(
    IAtlasOpsCapabilityRepository<MaintenanceWindowOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly MaintenanceWindowOptimizationValidator validator = new();
    private readonly MaintenanceWindowOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MaintenanceWindowOptimizationChanged>> ExecuteAsync(
        UpdateMaintenanceWindowOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MaintenanceWindowOptimizationChanged>.Invalid(issues);
        }

        MaintenanceWindowOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MaintenanceWindowOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MaintenanceWindowOptimizationChanged>.Invalid(
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

        MaintenanceWindowOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MaintenanceWindowOptimizationChanged>.Success(changed);
    }
}