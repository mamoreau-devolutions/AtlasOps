namespace AtlasOps.Features.Kubernetes.KubernetesServiceGovernance;

using AtlasOps.Features;

public sealed class KubernetesServiceGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesServiceGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesServiceGovernanceValidator validator = new();
    private readonly KubernetesServiceGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesServiceGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesServiceGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesServiceGovernanceChanged>.Invalid(issues);
        }

        KubernetesServiceGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesServiceGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesServiceGovernanceChanged>.Invalid(
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

        KubernetesServiceGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesServiceGovernanceChanged>.Success(changed);
    }
}