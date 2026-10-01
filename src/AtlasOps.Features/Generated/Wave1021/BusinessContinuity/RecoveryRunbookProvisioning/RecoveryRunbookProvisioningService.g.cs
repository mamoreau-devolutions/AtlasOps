namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookProvisioning;

using AtlasOps.Features;

public sealed class RecoveryRunbookProvisioningService(
    IAtlasOpsCapabilityRepository<RecoveryRunbookProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryRunbookProvisioningValidator validator = new();
    private readonly RecoveryRunbookProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryRunbookProvisioningChanged>> ExecuteAsync(
        UpdateRecoveryRunbookProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryRunbookProvisioningChanged>.Invalid(issues);
        }

        RecoveryRunbookProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryRunbookProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryRunbookProvisioningChanged>.Invalid(
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

        RecoveryRunbookProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryRunbookProvisioningChanged>.Success(changed);
    }
}