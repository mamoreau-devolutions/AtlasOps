namespace AtlasOps.Features.Architecture.ArchitectureDependencyRecovery;

using AtlasOps.Features;

public sealed class ArchitectureDependencyRecoveryService(
    IAtlasOpsCapabilityRepository<ArchitectureDependencyRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ArchitectureDependencyRecoveryValidator validator = new();
    private readonly ArchitectureDependencyRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ArchitectureDependencyRecoveryChanged>> ExecuteAsync(
        UpdateArchitectureDependencyRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ArchitectureDependencyRecoveryChanged>.Invalid(issues);
        }

        ArchitectureDependencyRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ArchitectureDependencyRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ArchitectureDependencyRecoveryChanged>.Invalid(
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

        ArchitectureDependencyRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ArchitectureDependencyRecoveryChanged>.Success(changed);
    }
}