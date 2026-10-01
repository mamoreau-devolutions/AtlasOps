namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceMonitoring;

using AtlasOps.Features;

public sealed class KubernetesNamespaceMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesNamespaceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesNamespaceMonitoringValidator validator = new();
    private readonly KubernetesNamespaceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesNamespaceMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesNamespaceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesNamespaceMonitoringChanged>.Invalid(issues);
        }

        KubernetesNamespaceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesNamespaceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesNamespaceMonitoringChanged>.Invalid(
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

        KubernetesNamespaceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesNamespaceMonitoringChanged>.Success(changed);
    }
}