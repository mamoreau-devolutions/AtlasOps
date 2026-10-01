namespace AtlasOps.Features.Security.SecurityBaselineMonitoring;

using AtlasOps.Features;

public sealed class SecurityBaselineMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityBaselineMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBaselineMonitoringValidator validator = new();
    private readonly SecurityBaselineMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBaselineMonitoringChanged>> ExecuteAsync(
        UpdateSecurityBaselineMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBaselineMonitoringChanged>.Invalid(issues);
        }

        SecurityBaselineMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBaselineMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBaselineMonitoringChanged>.Invalid(
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

        SecurityBaselineMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBaselineMonitoringChanged>.Success(changed);
    }
}