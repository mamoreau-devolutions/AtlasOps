namespace AtlasOps.Features.Kubernetes.KubernetesServiceProvisioning;

using AtlasOps.Features;

public sealed class KubernetesServiceProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesServiceProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesServiceProvisioningValidator validator = new();
    private readonly KubernetesServiceProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesServiceProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesServiceProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesServiceProvisioningChanged>.Invalid(issues);
        }

        KubernetesServiceProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesServiceProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesServiceProvisioningChanged>.Invalid(
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

        KubernetesServiceProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesServiceProvisioningChanged>.Success(changed);
    }
}