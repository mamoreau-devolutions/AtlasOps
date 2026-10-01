namespace AtlasOps.Features.Data.DataRetentionProvisioning;

using AtlasOps.Features;

public sealed class DataRetentionProvisioningService(
    IAtlasOpsCapabilityRepository<DataRetentionProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataRetentionProvisioningValidator validator = new();
    private readonly DataRetentionProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataRetentionProvisioningChanged>> ExecuteAsync(
        UpdateDataRetentionProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataRetentionProvisioningChanged>.Invalid(issues);
        }

        DataRetentionProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataRetentionProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataRetentionProvisioningChanged>.Invalid(
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

        DataRetentionProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataRetentionProvisioningChanged>.Success(changed);
    }
}