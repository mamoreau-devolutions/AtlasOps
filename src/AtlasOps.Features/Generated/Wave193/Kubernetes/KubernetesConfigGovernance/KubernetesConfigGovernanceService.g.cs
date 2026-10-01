namespace AtlasOps.Features.Kubernetes.KubernetesConfigGovernance;

using AtlasOps.Features;

public sealed class KubernetesConfigGovernanceService(
    IAtlasOpsCapabilityRepository<KubernetesConfigGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesConfigGovernanceValidator validator = new();
    private readonly KubernetesConfigGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesConfigGovernanceChanged>> ExecuteAsync(
        UpdateKubernetesConfigGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesConfigGovernanceChanged>.Invalid(issues);
        }

        KubernetesConfigGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesConfigGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesConfigGovernanceChanged>.Invalid(
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

        KubernetesConfigGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesConfigGovernanceChanged>.Success(changed);
    }
}