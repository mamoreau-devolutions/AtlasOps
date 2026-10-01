namespace AtlasOps.Features.Kubernetes.KubernetesVolumeRecovery;

using AtlasOps.Features;

public sealed class KubernetesVolumeRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesVolumeRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesVolumeRecoveryValidator validator = new();
    private readonly KubernetesVolumeRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesVolumeRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesVolumeRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesVolumeRecoveryChanged>.Invalid(issues);
        }

        KubernetesVolumeRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesVolumeRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesVolumeRecoveryChanged>.Invalid(
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

        KubernetesVolumeRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesVolumeRecoveryChanged>.Success(changed);
    }
}