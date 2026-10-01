namespace AtlasOps.Features.Data.DataTransformProvisioning;

using AtlasOps.Features;

public sealed class DataTransformProvisioningService(
    IAtlasOpsCapabilityRepository<DataTransformProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataTransformProvisioningValidator validator = new();
    private readonly DataTransformProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataTransformProvisioningChanged>> ExecuteAsync(
        UpdateDataTransformProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataTransformProvisioningChanged>.Invalid(issues);
        }

        DataTransformProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataTransformProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataTransformProvisioningChanged>.Invalid(
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

        DataTransformProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataTransformProvisioningChanged>.Success(changed);
    }
}