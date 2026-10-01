namespace AtlasOps.Features.Kubernetes.KubernetesPodProvisioning;

using AtlasOps.Features;

public sealed class KubernetesPodProvisioningService(
    IAtlasOpsCapabilityRepository<KubernetesPodProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesPodProvisioningValidator validator = new();
    private readonly KubernetesPodProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesPodProvisioningChanged>> ExecuteAsync(
        UpdateKubernetesPodProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesPodProvisioningChanged>.Invalid(issues);
        }

        KubernetesPodProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesPodProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesPodProvisioningChanged>.Invalid(
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

        KubernetesPodProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesPodProvisioningChanged>.Success(changed);
    }
}