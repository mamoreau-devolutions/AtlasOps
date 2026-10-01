namespace AtlasOps.Features.Kubernetes.KubernetesPodOptimization;

using AtlasOps.Features;

public sealed class KubernetesPodOptimizationService(
    IAtlasOpsCapabilityRepository<KubernetesPodOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesPodOptimizationValidator validator = new();
    private readonly KubernetesPodOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesPodOptimizationChanged>> ExecuteAsync(
        UpdateKubernetesPodOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesPodOptimizationChanged>.Invalid(issues);
        }

        KubernetesPodOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesPodOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesPodOptimizationChanged>.Invalid(
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

        KubernetesPodOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesPodOptimizationChanged>.Success(changed);
    }
}