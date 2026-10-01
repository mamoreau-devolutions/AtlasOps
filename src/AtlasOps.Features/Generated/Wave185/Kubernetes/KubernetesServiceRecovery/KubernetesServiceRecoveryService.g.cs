namespace AtlasOps.Features.Kubernetes.KubernetesServiceRecovery;

using AtlasOps.Features;

public sealed class KubernetesServiceRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesServiceRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesServiceRecoveryValidator validator = new();
    private readonly KubernetesServiceRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesServiceRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesServiceRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesServiceRecoveryChanged>.Invalid(issues);
        }

        KubernetesServiceRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesServiceRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesServiceRecoveryChanged>.Invalid(
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

        KubernetesServiceRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesServiceRecoveryChanged>.Success(changed);
    }
}