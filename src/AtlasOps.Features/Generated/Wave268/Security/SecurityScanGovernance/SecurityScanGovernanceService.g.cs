namespace AtlasOps.Features.Security.SecurityScanGovernance;

using AtlasOps.Features;

public sealed class SecurityScanGovernanceService(
    IAtlasOpsCapabilityRepository<SecurityScanGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityScanGovernanceValidator validator = new();
    private readonly SecurityScanGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityScanGovernanceChanged>> ExecuteAsync(
        UpdateSecurityScanGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityScanGovernanceChanged>.Invalid(issues);
        }

        SecurityScanGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityScanGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityScanGovernanceChanged>.Invalid(
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

        SecurityScanGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityScanGovernanceChanged>.Success(changed);
    }
}