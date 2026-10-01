namespace AtlasOps.Features.Kubernetes.KubernetesPodMonitoring;

using AtlasOps.Features;

public sealed class KubernetesPodMonitoringService(
    IAtlasOpsCapabilityRepository<KubernetesPodMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesPodMonitoringValidator validator = new();
    private readonly KubernetesPodMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesPodMonitoringChanged>> ExecuteAsync(
        UpdateKubernetesPodMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesPodMonitoringChanged>.Invalid(issues);
        }

        KubernetesPodMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesPodMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesPodMonitoringChanged>.Invalid(
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

        KubernetesPodMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesPodMonitoringChanged>.Success(changed);
    }
}