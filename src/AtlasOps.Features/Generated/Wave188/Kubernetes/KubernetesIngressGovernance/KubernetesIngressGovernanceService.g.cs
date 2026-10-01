namespace AtlasOps.Features.Kubernetes.KubernetesIngressGovernance;

using AtlasOps.Features;

public sealed class KubernetesIngressGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesIngressGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesIngressGovernanceValidator validator = new();
    private readonly KubernetesIngressGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesIngressGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesIngressGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesIngressGovernanceChanged>.Invalid(issues);
        }

        KubernetesIngressGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesIngressGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesIngressGovernanceChanged>.Invalid(
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

        KubernetesIngressGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesIngressGovernanceChanged>.Success(changed);
    }
}