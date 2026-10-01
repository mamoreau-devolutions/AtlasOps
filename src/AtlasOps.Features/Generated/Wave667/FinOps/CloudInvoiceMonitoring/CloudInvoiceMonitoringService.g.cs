namespace AtlasOps.Features.FinOps.CloudInvoiceMonitoring;

using AtlasOps.Features;

public sealed class CloudInvoiceMonitoringService(
    IAtlasOpsCapabilityRepository<CloudInvoiceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudInvoiceMonitoringValidator validator = new();
    private readonly CloudInvoiceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudInvoiceMonitoringChanged>> ExecuteAsync(
        UpdateCloudInvoiceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudInvoiceMonitoringChanged>.Invalid(issues);
        }

        CloudInvoiceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudInvoiceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudInvoiceMonitoringChanged>.Invalid(
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

        CloudInvoiceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudInvoiceMonitoringChanged>.Success(changed);
    }
}