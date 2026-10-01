namespace AtlasOps.Features.Observability.MetricSourceProvisioning;

using AtlasOps.Features;

public sealed class MetricSourceProvisioningService(
    IAtlasOpsCapabilityRepository<MetricSourceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricSourceProvisioningValidator validator = new();
    private readonly MetricSourceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricSourceProvisioningChanged>> ExecuteAsync(
        UpdateMetricSourceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricSourceProvisioningChanged>.Invalid(issues);
        }

        MetricSourceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricSourceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricSourceProvisioningChanged>.Invalid(
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

        MetricSourceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricSourceProvisioningChanged>.Success(changed);
    }
}