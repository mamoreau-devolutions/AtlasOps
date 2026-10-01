namespace AtlasOps.Features.Kubernetes.KubernetesIngressOptimization;

using AtlasOps.Features;

public sealed class KubernetesIngressOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesIngressOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesIngressOptimizationValidator validator = new();
    private readonly KubernetesIngressOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesIngressOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesIngressOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesIngressOptimizationChanged>.Invalid(issues);
        }

        KubernetesIngressOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesIngressOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesIngressOptimizationChanged>.Invalid(
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

        KubernetesIngressOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesIngressOptimizationChanged>.Success(changed);
    }
}