namespace AtlasOps.Features.Kubernetes.KubernetesConfigOptimization;

using AtlasOps.Features;

public sealed class KubernetesConfigOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesConfigOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesConfigOptimizationValidator validator = new();
    private readonly KubernetesConfigOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesConfigOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesConfigOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesConfigOptimizationChanged>.Invalid(issues);
        }

        KubernetesConfigOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesConfigOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesConfigOptimizationChanged>.Invalid(
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

        KubernetesConfigOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesConfigOptimizationChanged>.Success(changed);
    }
}