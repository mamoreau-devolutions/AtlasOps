namespace AtlasOps.Features.Kubernetes.KubernetesOperatorMonitoring;

using AtlasOps.Features;

public sealed class KubernetesOperatorMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesOperatorMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesOperatorMonitoringValidator validator = new();
    private readonly KubernetesOperatorMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesOperatorMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesOperatorMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesOperatorMonitoringChanged>.Invalid(issues);
        }

        KubernetesOperatorMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesOperatorMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesOperatorMonitoringChanged>.Invalid(
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

        KubernetesOperatorMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesOperatorMonitoringChanged>.Success(changed);
    }
}