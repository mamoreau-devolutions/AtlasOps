namespace AtlasOps.Features.Kubernetes.KubernetesClusterMonitoring;

using AtlasOps.Features;

public sealed class KubernetesClusterMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesClusterMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesClusterMonitoringValidator validator = new();
    private readonly KubernetesClusterMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesClusterMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesClusterMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesClusterMonitoringChanged>.Invalid(issues);
        }

        KubernetesClusterMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesClusterMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesClusterMonitoringChanged>.Invalid(
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

        KubernetesClusterMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesClusterMonitoringChanged>.Success(changed);
    }
}