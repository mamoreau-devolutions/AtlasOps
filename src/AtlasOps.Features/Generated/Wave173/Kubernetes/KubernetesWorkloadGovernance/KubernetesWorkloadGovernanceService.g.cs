namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadGovernance;

using AtlasOps.Features;

public sealed class KubernetesWorkloadGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesWorkloadGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesWorkloadGovernanceValidator validator = new();
    private readonly KubernetesWorkloadGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesWorkloadGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesWorkloadGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesWorkloadGovernanceChanged>.Invalid(issues);
        }

        KubernetesWorkloadGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesWorkloadGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesWorkloadGovernanceChanged>.Invalid(
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

        KubernetesWorkloadGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesWorkloadGovernanceChanged>.Success(changed);
    }
}