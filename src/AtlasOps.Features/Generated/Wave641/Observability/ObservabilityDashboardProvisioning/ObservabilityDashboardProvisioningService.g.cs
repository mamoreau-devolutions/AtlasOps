namespace AtlasOps.Features.Observability.ObservabilityDashboardProvisioning;

using AtlasOps.Features;

public sealed class ObservabilityDashboardProvisioningService(
    IAtlasOpsCapabilityRepository<ObservabilityDashboardProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityDashboardProvisioningValidator validator = new();
    private readonly ObservabilityDashboardProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityDashboardProvisioningChanged>> ExecuteAsync(
        UpdateObservabilityDashboardProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityDashboardProvisioningChanged>.Invalid(issues);
        }

        ObservabilityDashboardProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityDashboardProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityDashboardProvisioningChanged>.Invalid(
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

        ObservabilityDashboardProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityDashboardProvisioningChanged>.Success(changed);
    }
}