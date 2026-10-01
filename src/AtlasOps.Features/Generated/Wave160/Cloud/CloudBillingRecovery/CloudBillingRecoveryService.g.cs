namespace AtlasOps.Features.Cloud.CloudBillingRecovery;

using AtlasOps.Features;

public sealed class CloudBillingRecoveryService(
    IAtlasOpsCapabilityRepository<CloudBillingRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudBillingRecoveryValidator validator = new();
    private readonly CloudBillingRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudBillingRecoveryChanged>> ExecuteAsync(
        UpdateCloudBillingRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudBillingRecoveryChanged>.Invalid(issues);
        }

        CloudBillingRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudBillingRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudBillingRecoveryChanged>.Invalid(
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

        CloudBillingRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudBillingRecoveryChanged>.Success(changed);
    }
}