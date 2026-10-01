namespace AtlasOps.Features.Kubernetes.KubernetesIngressProvisioning;

using AtlasOps.Features;

public sealed class KubernetesIngressProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesIngressProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesIngressProvisioningValidator validator = new();
    private readonly KubernetesIngressProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesIngressProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesIngressProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesIngressProvisioningChanged>.Invalid(issues);
        }

        KubernetesIngressProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesIngressProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesIngressProvisioningChanged>.Invalid(
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

        KubernetesIngressProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesIngressProvisioningChanged>.Success(changed);
    }
}