namespace AtlasOps.Features.Kubernetes.KubernetesConfigMonitoring;

using AtlasOps.Features;

public sealed class KubernetesConfigMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesConfigMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesConfigMonitoringValidator validator = new();
    private readonly KubernetesConfigMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesConfigMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesConfigMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesConfigMonitoringChanged>.Invalid(issues);
        }

        KubernetesConfigMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesConfigMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesConfigMonitoringChanged>.Invalid(
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

        KubernetesConfigMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesConfigMonitoringChanged>.Success(changed);
    }
}