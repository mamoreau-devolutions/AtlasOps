namespace AtlasOps.Features.Kubernetes.KubernetesClusterProvisioning;

using AtlasOps.Features;

public sealed class KubernetesClusterProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesClusterProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesClusterProvisioningValidator validator = new();
    private readonly KubernetesClusterProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesClusterProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesClusterProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesClusterProvisioningChanged>.Invalid(issues);
        }

        KubernetesClusterProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesClusterProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesClusterProvisioningChanged>.Invalid(
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

        KubernetesClusterProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesClusterProvisioningChanged>.Success(changed);
    }
}