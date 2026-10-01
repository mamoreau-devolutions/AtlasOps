namespace AtlasOps.Features.Kubernetes.KubernetesVolumeOptimization;

using AtlasOps.Features;

public sealed class KubernetesVolumeOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesVolumeOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesVolumeOptimizationValidator validator = new();
    private readonly KubernetesVolumeOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesVolumeOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesVolumeOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesVolumeOptimizationChanged>.Invalid(issues);
        }

        KubernetesVolumeOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesVolumeOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesVolumeOptimizationChanged>.Invalid(
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

        KubernetesVolumeOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesVolumeOptimizationChanged>.Success(changed);
    }
}