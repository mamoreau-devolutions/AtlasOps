namespace AtlasOps.Features.Kubernetes.KubernetesConfigProvisioning;

using AtlasOps.Features;

public sealed class KubernetesConfigProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesConfigProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesConfigProvisioningValidator validator = new();
    private readonly KubernetesConfigProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesConfigProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesConfigProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesConfigProvisioningChanged>.Invalid(issues);
        }

        KubernetesConfigProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesConfigProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesConfigProvisioningChanged>.Invalid(
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

        KubernetesConfigProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesConfigProvisioningChanged>.Success(changed);
    }
}