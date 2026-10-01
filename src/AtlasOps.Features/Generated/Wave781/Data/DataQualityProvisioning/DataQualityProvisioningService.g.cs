namespace AtlasOps.Features.Data.DataQualityProvisioning;

using AtlasOps.Features;

public sealed class DataQualityProvisioningService(
    IAtlasOpsCapabilityRepository<DataQualityProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataQualityProvisioningValidator validator = new();
    private readonly DataQualityProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataQualityProvisioningChanged>> ExecuteAsync(
        UpdateDataQualityProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataQualityProvisioningChanged>.Invalid(issues);
        }

        DataQualityProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataQualityProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataQualityProvisioningChanged>.Invalid(
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

        DataQualityProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataQualityProvisioningChanged>.Success(changed);
    }
}