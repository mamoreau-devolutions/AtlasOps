namespace AtlasOps.Features.Security.SecurityPatchGovernance;

using AtlasOps.Features;

public sealed class SecurityPatchGovernanceService(
    IAtlasOpsCapabilityRepository<SecurityPatchGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityPatchGovernanceValidator validator = new();
    private readonly SecurityPatchGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityPatchGovernanceChanged>> ExecuteAsync(
        UpdateSecurityPatchGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityPatchGovernanceChanged>.Invalid(issues);
        }

        SecurityPatchGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityPatchGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityPatchGovernanceChanged>.Invalid(
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

        SecurityPatchGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityPatchGovernanceChanged>.Success(changed);
    }
}