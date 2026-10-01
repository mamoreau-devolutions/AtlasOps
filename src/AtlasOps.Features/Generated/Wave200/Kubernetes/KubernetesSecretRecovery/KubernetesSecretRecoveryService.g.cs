namespace AtlasOps.Features.Kubernetes.KubernetesSecretRecovery;

using AtlasOps.Features;

public sealed class KubernetesSecretRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesSecretRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesSecretRecoveryValidator validator = new();
    private readonly KubernetesSecretRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesSecretRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesSecretRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesSecretRecoveryChanged>.Invalid(issues);
        }

        KubernetesSecretRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesSecretRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesSecretRecoveryChanged>.Invalid(
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

        KubernetesSecretRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesSecretRecoveryChanged>.Success(changed);
    }
}