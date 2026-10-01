namespace AtlasOps.Features.Kubernetes.KubernetesOperatorProvisioning;

using AtlasOps.Features;

public sealed class KubernetesOperatorProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesOperatorProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesOperatorProvisioningValidator validator = new();
    private readonly KubernetesOperatorProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesOperatorProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesOperatorProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesOperatorProvisioningChanged>.Invalid(issues);
        }

        KubernetesOperatorProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesOperatorProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesOperatorProvisioningChanged>.Invalid(
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

        KubernetesOperatorProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesOperatorProvisioningChanged>.Success(changed);
    }
}