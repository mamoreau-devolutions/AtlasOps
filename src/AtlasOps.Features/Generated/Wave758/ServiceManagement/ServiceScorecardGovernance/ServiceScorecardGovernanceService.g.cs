namespace AtlasOps.Features.ServiceManagement.ServiceScorecardGovernance;

using AtlasOps.Features;

public sealed class ServiceScorecardGovernanceService(
    IAtlasOpsCapabilityRepository<ServiceScorecardGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceScorecardGovernanceValidator validator = new();
    private readonly ServiceScorecardGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceScorecardGovernanceChanged>> ExecuteAsync(
        UpdateServiceScorecardGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceScorecardGovernanceChanged>.Invalid(issues);
        }

        ServiceScorecardGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceScorecardGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceScorecardGovernanceChanged>.Invalid(
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

        ServiceScorecardGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceScorecardGovernanceChanged>.Success(changed);
    }
}