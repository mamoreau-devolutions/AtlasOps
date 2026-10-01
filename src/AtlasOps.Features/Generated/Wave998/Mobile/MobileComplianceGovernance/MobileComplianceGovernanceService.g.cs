namespace AtlasOps.Features.Mobile.MobileComplianceGovernance;

using AtlasOps.Features;

public sealed class MobileComplianceGovernanceService(
    IAtlasOpsCapabilityRepository<MobileComplianceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly MobileComplianceGovernanceValidator validator = new();
    private readonly MobileComplianceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<MobileComplianceGovernanceChanged>> ExecuteAsync(
        UpdateMobileComplianceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MobileComplianceGovernanceChanged>.Invalid(issues);
        }

        MobileComplianceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MobileComplianceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MobileComplianceGovernanceChanged>.Invalid(
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

        MobileComplianceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MobileComplianceGovernanceChanged>.Success(changed);
    }
}