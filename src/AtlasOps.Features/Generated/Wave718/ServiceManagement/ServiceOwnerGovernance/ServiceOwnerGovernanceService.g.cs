namespace AtlasOps.Features.ServiceManagement.ServiceOwnerGovernance;

using AtlasOps.Features;

public sealed class ServiceOwnerGovernanceService(
    IAtlasOpsCapabilityRepository<ServiceOwnerGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ServiceOwnerGovernanceValidator validator = new();
    private readonly ServiceOwnerGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ServiceOwnerGovernanceChanged>> ExecuteAsync(
        UpdateServiceOwnerGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ServiceOwnerGovernanceChanged>.Invalid(issues);
        }

        ServiceOwnerGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ServiceOwnerGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ServiceOwnerGovernanceChanged>.Invalid(
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

        ServiceOwnerGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ServiceOwnerGovernanceChanged>.Success(changed);
    }
}