namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowRecovery;

using AtlasOps.Features;

public sealed class MaintenanceWindowRecoveryService(
    IAtlasOpsCapabilityRepository<MaintenanceWindowRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MaintenanceWindowRecoveryValidator validator = new();
    private readonly MaintenanceWindowRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MaintenanceWindowRecoveryChanged>> ExecuteAsync(
        UpdateMaintenanceWindowRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MaintenanceWindowRecoveryChanged>.Invalid(issues);
        }

        MaintenanceWindowRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MaintenanceWindowRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MaintenanceWindowRecoveryChanged>.Invalid(
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

        MaintenanceWindowRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MaintenanceWindowRecoveryChanged>.Success(changed);
    }
}