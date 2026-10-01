namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadOptimization;

using AtlasOps.Features;

public sealed class KubernetesWorkloadOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesWorkloadOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesWorkloadOptimizationValidator validator = new();
    private readonly KubernetesWorkloadOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesWorkloadOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesWorkloadOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesWorkloadOptimizationChanged>.Invalid(issues);
        }

        KubernetesWorkloadOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesWorkloadOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesWorkloadOptimizationChanged>.Invalid(
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

        KubernetesWorkloadOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesWorkloadOptimizationChanged>.Success(changed);
    }
}