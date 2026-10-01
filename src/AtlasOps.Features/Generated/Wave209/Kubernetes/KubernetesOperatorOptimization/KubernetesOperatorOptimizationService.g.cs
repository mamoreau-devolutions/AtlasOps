namespace AtlasOps.Features.Kubernetes.KubernetesOperatorOptimization;

using AtlasOps.Features;

public sealed class KubernetesOperatorOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesOperatorOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesOperatorOptimizationValidator validator = new();
    private readonly KubernetesOperatorOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesOperatorOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesOperatorOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesOperatorOptimizationChanged>.Invalid(issues);
        }

        KubernetesOperatorOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesOperatorOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesOperatorOptimizationChanged>.Invalid(
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

        KubernetesOperatorOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesOperatorOptimizationChanged>.Success(changed);
    }
}