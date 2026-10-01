namespace AtlasOps.Features.Kubernetes.KubernetesIngressRecovery;

using AtlasOps.Features;

public sealed class KubernetesIngressRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesIngressRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesIngressRecoveryValidator validator = new();
    private readonly KubernetesIngressRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesIngressRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesIngressRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesIngressRecoveryChanged>.Invalid(issues);
        }

        KubernetesIngressRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesIngressRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesIngressRecoveryChanged>.Invalid(
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

        KubernetesIngressRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesIngressRecoveryChanged>.Success(changed);
    }
}