namespace AtlasOps.Features.Kubernetes.KubernetesPodGovernance;

using AtlasOps.Features;

public sealed class KubernetesPodGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesPodGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesPodGovernanceValidator validator = new();
    private readonly KubernetesPodGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesPodGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesPodGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesPodGovernanceChanged>.Invalid(issues);
        }

        KubernetesPodGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesPodGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesPodGovernanceChanged>.Invalid(
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

        KubernetesPodGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesPodGovernanceChanged>.Success(changed);
    }
}