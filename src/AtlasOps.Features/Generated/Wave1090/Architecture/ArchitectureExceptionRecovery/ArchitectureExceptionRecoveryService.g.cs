namespace AtlasOps.Features.Architecture.ArchitectureExceptionRecovery;

using AtlasOps.Features;

public sealed class ArchitectureExceptionRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureExceptionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureExceptionRecoveryValidator validator = new();
    private readonly ArchitectureExceptionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureExceptionRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureExceptionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureExceptionRecoveryChanged>.Invalid(issues);
        }

        ArchitectureExceptionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureExceptionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureExceptionRecoveryChanged>.Invalid(
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

        ArchitectureExceptionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureExceptionRecoveryChanged>.Success(changed);
    }
}