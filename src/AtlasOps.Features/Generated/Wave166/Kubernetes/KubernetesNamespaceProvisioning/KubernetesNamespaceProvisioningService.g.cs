namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceProvisioning;

using AtlasOps.Features;

public sealed class KubernetesNamespaceProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesNamespaceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesNamespaceProvisioningValidator validator = new();
    private readonly KubernetesNamespaceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesNamespaceProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesNamespaceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesNamespaceProvisioningChanged>.Invalid(issues);
        }

        KubernetesNamespaceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesNamespaceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesNamespaceProvisioningChanged>.Invalid(
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

        KubernetesNamespaceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesNamespaceProvisioningChanged>.Success(changed);
    }
}