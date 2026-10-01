namespace AtlasOps.Features.Kubernetes.KubernetesIngressMonitoring;

using AtlasOps.Features;

public sealed class KubernetesIngressMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesIngressMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesIngressMonitoringValidator validator = new();
    private readonly KubernetesIngressMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesIngressMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesIngressMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesIngressMonitoringChanged>.Invalid(issues);
        }

        KubernetesIngressMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesIngressMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesIngressMonitoringChanged>.Invalid(
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

        KubernetesIngressMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesIngressMonitoringChanged>.Success(changed);
    }
}