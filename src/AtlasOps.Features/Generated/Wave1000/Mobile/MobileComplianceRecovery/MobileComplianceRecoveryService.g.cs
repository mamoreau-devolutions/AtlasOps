namespace AtlasOps.Features.Mobile.MobileComplianceRecovery;

using AtlasOps.Features;

public sealed class MobileComplianceRecoveryService(
    IAtlasOpsCapabilityRepository<MobileComplianceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileComplianceRecoveryValidator validator = new();
    private readonly MobileComplianceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileComplianceRecoveryChanged>> ExecuteAsync(
        UpdateMobileComplianceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileComplianceRecoveryChanged>.Invalid(issues);
        }

        MobileComplianceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileComplianceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileComplianceRecoveryChanged>.Invalid(
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

        MobileComplianceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileComplianceRecoveryChanged>.Success(changed);
    }
}