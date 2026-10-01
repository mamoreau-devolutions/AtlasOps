namespace AtlasOps.Features.Kubernetes.KubernetesVolumeMonitoring;

using AtlasOps.Features;

public sealed class KubernetesVolumeMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesVolumeMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesVolumeMonitoringValidator validator = new();
    private readonly KubernetesVolumeMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesVolumeMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesVolumeMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesVolumeMonitoringChanged>.Invalid(issues);
        }

        KubernetesVolumeMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesVolumeMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesVolumeMonitoringChanged>.Invalid(
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

        KubernetesVolumeMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesVolumeMonitoringChanged>.Success(changed);
    }
}