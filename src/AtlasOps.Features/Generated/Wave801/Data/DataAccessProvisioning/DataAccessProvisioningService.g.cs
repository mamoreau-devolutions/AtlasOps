namespace AtlasOps.Features.Data.DataAccessProvisioning;

using AtlasOps.Features;

public sealed class DataAccessProvisioningService(
    IAtlasOpsCapabilityRepository<DataAccessProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly DataAccessProvisioningValidator validator = new();
    private readonly DataAccessProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<DataAccessProvisioningChanged>> ExecuteAsync(
        UpdateDataAccessProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<DataAccessProvisioningChanged>.Invalid(issues);
        }

        DataAccessProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new DataAccessProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<DataAccessProvisioningChanged>.Invalid(
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

        DataAccessProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<DataAccessProvisioningChanged>.Success(changed);
    }
}