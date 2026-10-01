namespace AtlasOps.Features.BusinessContinuity.RecoveryBackupProvisioning;

using AtlasOps.Features;

public sealed class RecoveryBackupProvisioningService(
    IAtlasOpsCapabilityRepository<RecoveryBackupProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryBackupProvisioningValidator validator = new();
    private readonly RecoveryBackupProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryBackupProvisioningChanged>> ExecuteAsync(
        UpdateRecoveryBackupProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryBackupProvisioningChanged>.Invalid(issues);
        }

        RecoveryBackupProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryBackupProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryBackupProvisioningChanged>.Invalid(
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

        RecoveryBackupProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryBackupProvisioningChanged>.Success(changed);
    }
}