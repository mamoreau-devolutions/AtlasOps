namespace AtlasOps.Features.Architecture.ArchitectureStandardRecovery;

using AtlasOps.Features;

public sealed class ArchitectureStandardRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureStandardRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureStandardRecoveryValidator validator = new();
    private readonly ArchitectureStandardRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureStandardRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureStandardRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureStandardRecoveryChanged>.Invalid(issues);
        }

        ArchitectureStandardRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureStandardRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureStandardRecoveryChanged>.Invalid(
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

        ArchitectureStandardRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureStandardRecoveryChanged>.Success(changed);
    }
}