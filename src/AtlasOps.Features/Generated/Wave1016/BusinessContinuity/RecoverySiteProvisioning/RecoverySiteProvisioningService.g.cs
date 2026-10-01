namespace AtlasOps.Features.BusinessContinuity.RecoverySiteProvisioning;

using AtlasOps.Features;

public sealed class RecoverySiteProvisioningService(
    IAtlasOpsCapabilityRepository<RecoverySiteProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoverySiteProvisioningValidator validator = new();
    private readonly RecoverySiteProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoverySiteProvisioningChanged>> ExecuteAsync(
        UpdateRecoverySiteProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoverySiteProvisioningChanged>.Invalid(issues);
        }

        RecoverySiteProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoverySiteProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoverySiteProvisioningChanged>.Invalid(
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

        RecoverySiteProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoverySiteProvisioningChanged>.Success(changed);
    }
}