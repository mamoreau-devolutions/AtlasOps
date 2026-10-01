namespace AtlasOps.Features.Delivery.SourceRepositoryRecovery;

using AtlasOps.Features;

public sealed class SourceRepositoryRecoveryService(
    IAtlasOpsCapabilityRepository<SourceRepositoryRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly SourceRepositoryRecoveryValidator validator = new();
    private readonly SourceRepositoryRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SourceRepositoryRecoveryChanged>> ExecuteAsync(
        UpdateSourceRepositoryRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SourceRepositoryRecoveryChanged>.Invalid(issues);
        }

        SourceRepositoryRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SourceRepositoryRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SourceRepositoryRecoveryChanged>.Invalid(
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

        SourceRepositoryRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SourceRepositoryRecoveryChanged>.Success(changed);
    }
}