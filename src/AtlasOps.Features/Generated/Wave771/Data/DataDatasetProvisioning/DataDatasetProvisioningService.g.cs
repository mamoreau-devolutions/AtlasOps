namespace AtlasOps.Features.Data.DataDatasetProvisioning;

using AtlasOps.Features;

public sealed class DataDatasetProvisioningService(
    IAtlasOpsCapabilityRepository<DataDatasetProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataDatasetProvisioningValidator validator = new();
    private readonly DataDatasetProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataDatasetProvisioningChanged>> ExecuteAsync(
        UpdateDataDatasetProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataDatasetProvisioningChanged>.Invalid(issues);
        }

        DataDatasetProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataDatasetProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataDatasetProvisioningChanged>.Invalid(
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

        DataDatasetProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataDatasetProvisioningChanged>.Success(changed);
    }
}