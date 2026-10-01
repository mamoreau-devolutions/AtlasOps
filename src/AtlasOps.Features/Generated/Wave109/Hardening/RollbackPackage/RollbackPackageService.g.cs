namespace AtlasOps.Features.Hardening.RollbackPackage;

using AtlasOps.Features;

public sealed class RollbackPackageService(
    IAtlasOpsCapabilityRepository<RollbackPackageItem> repository,
    TimeProvider timeProvider)
{
    private readonly RollbackPackageValidator validator = new();
    private readonly RollbackPackagePolicy policy = new();

    public async Task<AtlasOpsOperationResult<RollbackPackageChanged>> ExecuteAsync(
        UpdateRollbackPackageCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RollbackPackageChanged>.Invalid(issues);
        }

        RollbackPackageItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RollbackPackageItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RollbackPackageChanged>.Invalid(
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

        RollbackPackageChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RollbackPackageChanged>.Success(changed);
    }
}