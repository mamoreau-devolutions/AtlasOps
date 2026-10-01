namespace AtlasOps.Features.Security.SecurityExceptionMonitoring;

using AtlasOps.Features;

public sealed class SecurityExceptionMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityExceptionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityExceptionMonitoringValidator validator = new();
    private readonly SecurityExceptionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityExceptionMonitoringChanged>> ExecuteAsync(
        UpdateSecurityExceptionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityExceptionMonitoringChanged>.Invalid(issues);
        }

        SecurityExceptionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityExceptionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityExceptionMonitoringChanged>.Invalid(
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

        SecurityExceptionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityExceptionMonitoringChanged>.Success(changed);
    }
}