namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceProvisioning;

using AtlasOps.Features;

public sealed class RecoveryEvidenceProvisioningService(
    IAtlasOpsCapabilityRepository<RecoveryEvidenceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly RecoveryEvidenceProvisioningValidator validator = new();
    private readonly RecoveryEvidenceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RecoveryEvidenceProvisioningChanged>> ExecuteAsync(
        UpdateRecoveryEvidenceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RecoveryEvidenceProvisioningChanged>.Invalid(issues);
        }

        RecoveryEvidenceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RecoveryEvidenceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RecoveryEvidenceProvisioningChanged>.Invalid(
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

        RecoveryEvidenceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RecoveryEvidenceProvisioningChanged>.Success(changed);
    }
}