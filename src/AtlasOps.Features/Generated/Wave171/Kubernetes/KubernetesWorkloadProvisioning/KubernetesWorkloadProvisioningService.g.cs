namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadProvisioning;

using AtlasOps.Features;

public sealed class KubernetesWorkloadProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesWorkloadProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesWorkloadProvisioningValidator validator = new();
    private readonly KubernetesWorkloadProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesWorkloadProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesWorkloadProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesWorkloadProvisioningChanged>.Invalid(issues);
        }

        KubernetesWorkloadProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesWorkloadProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesWorkloadProvisioningChanged>.Invalid(
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

        KubernetesWorkloadProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesWorkloadProvisioningChanged>.Success(changed);
    }
}