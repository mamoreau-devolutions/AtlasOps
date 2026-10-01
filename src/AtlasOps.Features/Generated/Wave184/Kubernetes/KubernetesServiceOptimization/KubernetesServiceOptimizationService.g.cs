namespace AtlasOps.Features.Kubernetes.KubernetesServiceOptimization;

using AtlasOps.Features;

public sealed class KubernetesServiceOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesServiceOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesServiceOptimizationValidator validator = new();
    private readonly KubernetesServiceOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesServiceOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesServiceOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesServiceOptimizationChanged>.Invalid(issues);
        }

        KubernetesServiceOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesServiceOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesServiceOptimizationChanged>.Invalid(
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

        KubernetesServiceOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesServiceOptimizationChanged>.Success(changed);
    }
}