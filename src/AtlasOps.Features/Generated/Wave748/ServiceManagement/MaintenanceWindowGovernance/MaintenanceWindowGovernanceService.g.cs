namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowGovernance;

using AtlasOps.Features;

public sealed class MaintenanceWindowGovernanceService(
    IAtlasOpsCapabilityRepository<MaintenanceWindowGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MaintenanceWindowGovernanceValidator validator = new();
    private readonly MaintenanceWindowGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MaintenanceWindowGovernanceChanged>> ExecuteAsync(
        UpdateMaintenanceWindowGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MaintenanceWindowGovernanceChanged>.Invalid(issues);
        }

        MaintenanceWindowGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MaintenanceWindowGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MaintenanceWindowGovernanceChanged>.Invalid(
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

        MaintenanceWindowGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MaintenanceWindowGovernanceChanged>.Success(changed);
    }
}