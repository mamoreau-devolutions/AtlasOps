namespace AtlasOps.Features.Security.SecurityExceptionGovernance;

using AtlasOps.Features;

public sealed class SecurityExceptionGovernanceService(
    IAtlasOpsCapabilityRepository<SecurityExceptionGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityExceptionGovernanceValidator validator = new();
    private readonly SecurityExceptionGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityExceptionGovernanceChanged>> ExecuteAsync(
        UpdateSecurityExceptionGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityExceptionGovernanceChanged>.Invalid(issues);
        }

        SecurityExceptionGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityExceptionGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityExceptionGovernanceChanged>.Invalid(
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

        SecurityExceptionGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityExceptionGovernanceChanged>.Success(changed);
    }
}