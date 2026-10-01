namespace AtlasOps.Features.Kubernetes.KubernetesConfigRecovery;

using AtlasOps.Features;

public sealed class KubernetesConfigRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesConfigRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesConfigRecoveryValidator validator = new();
    private readonly KubernetesConfigRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesConfigRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesConfigRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesConfigRecoveryChanged>.Invalid(issues);
        }

        KubernetesConfigRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesConfigRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesConfigRecoveryChanged>.Invalid(
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

        KubernetesConfigRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesConfigRecoveryChanged>.Success(changed);
    }
}