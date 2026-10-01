namespace AtlasOps.Features.ServiceManagement.ServiceReviewGovernance;

using AtlasOps.Features;

public sealed class ServiceReviewGovernanceService(
    IAtlasOpsCapabilityRepository<ServiceReviewGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceReviewGovernanceValidator validator = new();
    private readonly ServiceReviewGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceReviewGovernanceChanged>> ExecuteAsync(
        UpdateServiceReviewGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceReviewGovernanceChanged>.Invalid(issues);
        }

        ServiceReviewGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceReviewGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceReviewGovernanceChanged>.Invalid(
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

        ServiceReviewGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceReviewGovernanceChanged>.Success(changed);
    }
}