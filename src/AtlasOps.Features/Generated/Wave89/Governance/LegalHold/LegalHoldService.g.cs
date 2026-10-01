namespace AtlasOps.Features.Governance.LegalHold;

using AtlasOps.Features;

public sealed class LegalHoldService(
    IAtlasOpsCapabilityRepository<LegalHoldItem> repository,
    TimeProvider timeProvider)
{
    private readonly LegalHoldValidator validator = new();
    private readonly LegalHoldPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LegalHoldChanged>> ExecuteAsync(
        UpdateLegalHoldCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LegalHoldChanged>.Invalid(issues);
        }

        LegalHoldItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LegalHoldItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LegalHoldChanged>.Invalid(
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

        LegalHoldChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LegalHoldChanged>.Success(changed);
    }
}