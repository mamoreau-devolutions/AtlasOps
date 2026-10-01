namespace AtlasOps.Features.Compute.ComputeTemplateRecovery;

using AtlasOps.Features;

public sealed class ComputeTemplateRecoveryService(
    IAtlasOpsCapabilityRepository<ComputeTemplateRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeTemplateRecoveryValidator validator = new();
    private readonly ComputeTemplateRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeTemplateRecoveryChanged>> ExecuteAsync(
        UpdateComputeTemplateRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeTemplateRecoveryChanged>.Invalid(issues);
        }

        ComputeTemplateRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeTemplateRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeTemplateRecoveryChanged>.Invalid(
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

        ComputeTemplateRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeTemplateRecoveryChanged>.Success(changed);
    }
}