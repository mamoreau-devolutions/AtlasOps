namespace AtlasOps.Features.Cloud.CloudBillingOptimization;

using AtlasOps.Features;

public sealed class CloudBillingOptimizationService(
    IAtlasOpsCapabilityRepository<CloudBillingOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CloudBillingOptimizationValidator validator = new();
    private readonly CloudBillingOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CloudBillingOptimizationChanged>> ExecuteAsync(
        UpdateCloudBillingOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CloudBillingOptimizationChanged>.Invalid(issues);
        }

        CloudBillingOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CloudBillingOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CloudBillingOptimizationChanged>.Invalid(
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

        CloudBillingOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CloudBillingOptimizationChanged>.Success(changed);
    }
}