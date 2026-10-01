namespace AtlasOps.Features.Kubernetes.KubernetesOperatorGovernance;

using AtlasOps.Features;

public sealed class KubernetesOperatorGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesOperatorGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesOperatorGovernanceValidator validator = new();
    private readonly KubernetesOperatorGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesOperatorGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesOperatorGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesOperatorGovernanceChanged>.Invalid(issues);
        }

        KubernetesOperatorGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesOperatorGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesOperatorGovernanceChanged>.Invalid(
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

        KubernetesOperatorGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesOperatorGovernanceChanged>.Success(changed);
    }
}