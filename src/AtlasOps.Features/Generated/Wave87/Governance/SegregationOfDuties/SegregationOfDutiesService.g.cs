namespace AtlasOps.Features.Governance.SegregationOfDuties;

using AtlasOps.Features;

public sealed class SegregationOfDutiesService(
    IAtlasOpsCapabilityRepository<SegregationOfDutiesItem> repository,
    TimeProvider timeProvider)
{
    private readonly SegregationOfDutiesValidator validator = new();
    private readonly SegregationOfDutiesPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SegregationOfDutiesChanged>> ExecuteAsync(
        UpdateSegregationOfDutiesCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SegregationOfDutiesChanged>.Invalid(issues);
        }

        SegregationOfDutiesItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SegregationOfDutiesItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SegregationOfDutiesChanged>.Invalid(
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

        SegregationOfDutiesChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SegregationOfDutiesChanged>.Success(changed);
    }
}