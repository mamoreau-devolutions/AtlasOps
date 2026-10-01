namespace AtlasOps.Features.Kubernetes.KubernetesClusterRecovery;

using AtlasOps.Features;

public sealed class KubernetesClusterRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesClusterRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesClusterRecoveryValidator validator = new();
    private readonly KubernetesClusterRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesClusterRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesClusterRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesClusterRecoveryChanged>.Invalid(issues);
        }

        KubernetesClusterRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesClusterRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesClusterRecoveryChanged>.Invalid(
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

        KubernetesClusterRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesClusterRecoveryChanged>.Success(changed);
    }
}