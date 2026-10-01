namespace AtlasOps.Features.Connections.KubernetesContext;

using AtlasOps.Features;

public sealed class KubernetesContextService(
    IAtlasOpsCapabilityRepository<KubernetesContextItem> repository,
    TimeProvider timeProvider)
{
    private readonly KubernetesContextValidator validator = new();
    private readonly KubernetesContextPolicy policy = new();

    public async Task<AtlasOpsOperationResult<KubernetesContextChanged>> ExecuteAsync(
        UpdateKubernetesContextCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<KubernetesContextChanged>.Invalid(issues);
        }

        KubernetesContextItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new KubernetesContextItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<KubernetesContextChanged>.Invalid(
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

        KubernetesContextChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<KubernetesContextChanged>.Success(changed);
    }
}