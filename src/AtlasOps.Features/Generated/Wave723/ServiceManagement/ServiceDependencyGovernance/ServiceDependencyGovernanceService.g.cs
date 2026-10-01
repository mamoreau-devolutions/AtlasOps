namespace AtlasOps.Features.ServiceManagement.ServiceDependencyGovernance;

using AtlasOps.Features;

public sealed class ServiceDependencyGovernanceService(
    IAtlasOpsCapabilityRepository<ServiceDependencyGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceDependencyGovernanceValidator validator = new();
    private readonly ServiceDependencyGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceDependencyGovernanceChanged>> ExecuteAsync(
        UpdateServiceDependencyGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceDependencyGovernanceChanged>.Invalid(issues);
        }

        ServiceDependencyGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceDependencyGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceDependencyGovernanceChanged>.Invalid(
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

        ServiceDependencyGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceDependencyGovernanceChanged>.Success(changed);
    }
}