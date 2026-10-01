namespace AtlasOps.Features.Kubernetes.KubernetesClusterGovernance;

using AtlasOps.Features;

public sealed class KubernetesClusterGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesClusterGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesClusterGovernanceValidator validator = new();
    private readonly KubernetesClusterGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesClusterGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesClusterGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesClusterGovernanceChanged>.Invalid(issues);
        }

        KubernetesClusterGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesClusterGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesClusterGovernanceChanged>.Invalid(
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

        KubernetesClusterGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesClusterGovernanceChanged>.Success(changed);
    }
}