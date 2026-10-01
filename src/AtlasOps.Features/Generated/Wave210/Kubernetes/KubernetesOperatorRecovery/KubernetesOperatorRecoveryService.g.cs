namespace AtlasOps.Features.Kubernetes.KubernetesOperatorRecovery;

using AtlasOps.Features;

public sealed class KubernetesOperatorRecoveryService(
    IAtlasOpsCapabilityRepository<KubernetesOperatorRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesOperatorRecoveryValidator validator = new();
    private readonly KubernetesOperatorRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesOperatorRecoveryChanged>> ExecuteAsync(
        UpdateKubernetesOperatorRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesOperatorRecoveryChanged>.Invalid(issues);
        }

        KubernetesOperatorRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesOperatorRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesOperatorRecoveryChanged>.Invalid(
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

        KubernetesOperatorRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesOperatorRecoveryChanged>.Success(changed);
    }
}