namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadMonitoring;

using AtlasOps.Features;

public sealed class KubernetesWorkloadMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesWorkloadMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesWorkloadMonitoringValidator validator = new();
    private readonly KubernetesWorkloadMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesWorkloadMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesWorkloadMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesWorkloadMonitoringChanged>.Invalid(issues);
        }

        KubernetesWorkloadMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesWorkloadMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesWorkloadMonitoringChanged>.Invalid(
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

        KubernetesWorkloadMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesWorkloadMonitoringChanged>.Success(changed);
    }
}