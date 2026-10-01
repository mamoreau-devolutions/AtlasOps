namespace AtlasOps.Features.Data.DataContractProvisioning;

using AtlasOps.Features;

public sealed class DataContractProvisioningService(
    IAtlasOpsCapabilityRepository<DataContractProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataContractProvisioningValidator validator = new();
    private readonly DataContractProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataContractProvisioningChanged>> ExecuteAsync(
        UpdateDataContractProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataContractProvisioningChanged>.Invalid(issues);
        }

        DataContractProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataContractProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataContractProvisioningChanged>.Invalid(
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

        DataContractProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataContractProvisioningChanged>.Success(changed);
    }
}