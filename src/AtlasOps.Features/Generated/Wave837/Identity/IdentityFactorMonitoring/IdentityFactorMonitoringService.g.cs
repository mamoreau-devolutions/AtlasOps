namespace AtlasOps.Features.Identity.IdentityFactorMonitoring;

using AtlasOps.Features;

public sealed class IdentityFactorMonitoringService(
    IAtlasOpsCapabilityRepository<IdentityFactorMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityFactorMonitoringValidator validator = new();
    private readonly IdentityFactorMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityFactorMonitoringChanged>> ExecuteAsync(
        UpdateIdentityFactorMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityFactorMonitoringChanged>.Invalid(issues);
        }

        IdentityFactorMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityFactorMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityFactorMonitoringChanged>.Invalid(
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

        IdentityFactorMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityFactorMonitoringChanged>.Success(changed);
    }
}