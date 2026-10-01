namespace AtlasOps.Features.Cloud.AwsAccountProvisioning;

using AtlasOps.Features;

public sealed class AwsAccountProvisioningService(
    IAtlasOpsCapabilityRepository<AwsAccountProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly AwsAccountProvisioningValidator validator = new();
    private readonly AwsAccountProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<AwsAccountProvisioningChanged>> ExecuteAsync(
        UpdateAwsAccountProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<AwsAccountProvisioningChanged>.Invalid(issues);
        }

        AwsAccountProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new AwsAccountProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<AwsAccountProvisioningChanged>.Invalid(
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

        AwsAccountProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<AwsAccountProvisioningChanged>.Success(changed);
    }
}