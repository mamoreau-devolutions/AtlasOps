namespace AtlasOps.Features.Security.SecurityCertificateGovernance;

using AtlasOps.Features;

public sealed class SecurityCertificateGovernanceService(
    IAtlasOpsCapabilityRepository<SecurityCertificateGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityCertificateGovernanceValidator validator = new();
    private readonly SecurityCertificateGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityCertificateGovernanceChanged>> ExecuteAsync(
        UpdateSecurityCertificateGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityCertificateGovernanceChanged>.Invalid(issues);
        }

        SecurityCertificateGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityCertificateGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityCertificateGovernanceChanged>.Invalid(
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

        SecurityCertificateGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityCertificateGovernanceChanged>.Success(changed);
    }
}