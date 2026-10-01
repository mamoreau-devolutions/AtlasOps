namespace AtlasOps.Features.Observability.MetricAlertProvisioning;

using AtlasOps.Features;

public sealed class MetricAlertProvisioningService(
    IAtlasOpsCapabilityRepository<MetricAlertProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricAlertProvisioningValidator validator = new();
    private readonly MetricAlertProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricAlertProvisioningChanged>> ExecuteAsync(
        UpdateMetricAlertProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricAlertProvisioningChanged>.Invalid(issues);
        }

        MetricAlertProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricAlertProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricAlertProvisioningChanged>.Invalid(
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

        MetricAlertProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricAlertProvisioningChanged>.Success(changed);
    }
}