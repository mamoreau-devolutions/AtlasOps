namespace AtlasOps.Features.Kubernetes.KubernetesVolumeProvisioning;

using AtlasOps.Features;

public sealed class KubernetesVolumeProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesVolumeProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesVolumeProvisioningValidator validator = new();
    private readonly KubernetesVolumeProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesVolumeProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesVolumeProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesVolumeProvisioningChanged>.Invalid(issues);
        }

        KubernetesVolumeProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesVolumeProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesVolumeProvisioningChanged>.Invalid(
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

        KubernetesVolumeProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesVolumeProvisioningChanged>.Success(changed);
    }
}