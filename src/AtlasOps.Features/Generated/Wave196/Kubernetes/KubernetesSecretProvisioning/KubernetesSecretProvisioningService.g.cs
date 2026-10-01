namespace AtlasOps.Features.Kubernetes.KubernetesSecretProvisioning;

using AtlasOps.Features;

public sealed class KubernetesSecretProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesSecretProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesSecretProvisioningValidator validator = new();
    private readonly KubernetesSecretProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesSecretProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesSecretProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesSecretProvisioningChanged>.Invalid(issues);
        }

        KubernetesSecretProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesSecretProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesSecretProvisioningChanged>.Invalid(
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

        KubernetesSecretProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesSecretProvisioningChanged>.Success(changed);
    }
}