namespace AtlasOps.Features.Kubernetes.KubernetesVolumeGovernance;

using AtlasOps.Features;

public sealed class KubernetesVolumeGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesVolumeGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesVolumeGovernanceValidator validator = new();
    private readonly KubernetesVolumeGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesVolumeGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesVolumeGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesVolumeGovernanceChanged>.Invalid(issues);
        }

        KubernetesVolumeGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesVolumeGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesVolumeGovernanceChanged>.Invalid(
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

        KubernetesVolumeGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesVolumeGovernanceChanged>.Success(changed);
    }
}