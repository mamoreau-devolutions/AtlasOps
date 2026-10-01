namespace AtlasOps.Features.ServiceManagement.ServiceRequestGovernance;

using AtlasOps.Features;

public sealed class ServiceRequestGovernanceService(
    IAtlasOpsCapabilityRepository<ServiceRequestGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceRequestGovernanceValidator validator = new();
    private readonly ServiceRequestGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceRequestGovernanceChanged>> ExecuteAsync(
        UpdateServiceRequestGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceRequestGovernanceChanged>.Invalid(issues);
        }

        ServiceRequestGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceRequestGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceRequestGovernanceChanged>.Invalid(
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

        ServiceRequestGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceRequestGovernanceChanged>.Success(changed);
    }
}