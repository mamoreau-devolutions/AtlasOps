namespace AtlasOps.Features.Kubernetes.KubernetesClusterOptimization;

using AtlasOps.Features;

public sealed class KubernetesClusterOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesClusterOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesClusterOptimizationValidator validator = new();
    private readonly KubernetesClusterOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesClusterOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesClusterOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesClusterOptimizationChanged>.Invalid(issues);
        }

        KubernetesClusterOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesClusterOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesClusterOptimizationChanged>.Invalid(
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

        KubernetesClusterOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesClusterOptimizationChanged>.Success(changed);
    }
}