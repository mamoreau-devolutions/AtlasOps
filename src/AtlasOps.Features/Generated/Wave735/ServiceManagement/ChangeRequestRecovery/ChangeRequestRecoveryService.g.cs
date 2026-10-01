namespace AtlasOps.Features.ServiceManagement.ChangeRequestRecovery;

using AtlasOps.Features;

public sealed class ChangeRequestRecoveryService(
    IAtlasOpsCapabilityRepository<ChangeRequestRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly ChangeRequestRecoveryValidator validator = new();
    private readonly ChangeRequestRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ChangeRequestRecoveryChanged>> ExecuteAsync(
        UpdateChangeRequestRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ChangeRequestRecoveryChanged>.Invalid(issues);
        }

        ChangeRequestRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ChangeRequestRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ChangeRequestRecoveryChanged>.Invalid(
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

        ChangeRequestRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ChangeRequestRecoveryChanged>.Success(changed);
    }
}