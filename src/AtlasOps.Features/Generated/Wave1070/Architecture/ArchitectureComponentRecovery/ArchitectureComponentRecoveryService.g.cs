namespace AtlasOps.Features.Architecture.ArchitectureComponentRecovery;

using AtlasOps.Features;

public sealed class ArchitectureComponentRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureComponentRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureComponentRecoveryValidator validator = new();
    private readonly ArchitectureComponentRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureComponentRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureComponentRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureComponentRecoveryChanged>.Invalid(issues);
        }

        ArchitectureComponentRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureComponentRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureComponentRecoveryChanged>.Invalid(
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

        ArchitectureComponentRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureComponentRecoveryChanged>.Success(changed);
    }
}