namespace AtlasOps.Features.Security.SecurityFindingMonitoring;

using AtlasOps.Features;

public sealed class SecurityFindingMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityFindingMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityFindingMonitoringValidator validator = new();
    private readonly SecurityFindingMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityFindingMonitoringChanged>> ExecuteAsync(
        UpdateSecurityFindingMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityFindingMonitoringChanged>.Invalid(issues);
        }

        SecurityFindingMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityFindingMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityFindingMonitoringChanged>.Invalid(
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

        SecurityFindingMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityFindingMonitoringChanged>.Success(changed);
    }
}