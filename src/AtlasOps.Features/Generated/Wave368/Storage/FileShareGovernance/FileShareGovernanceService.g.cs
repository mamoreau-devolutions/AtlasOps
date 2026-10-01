namespace AtlasOps.Features.Storage.FileShareGovernance;

using AtlasOps.Features;

public sealed class FileShareGovernanceService(
    IAtlasOpsCapabilityRepository<FileShareGovernanceItem> repository,
    TimeProvider timeProvider)
{
    private readonly FileShareGovernanceValidator validator = new();
    private readonly FileShareGovernancePolicy policy = new();

    public async Task<AtlasOpsOperationResult<FileShareGovernanceChanged>> ExecuteAsync(
        UpdateFileShareGovernanceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<FileShareGovernanceChanged>.Invalid(issues);
        }

        FileShareGovernanceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new FileShareGovernanceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<FileShareGovernanceChanged>.Invalid(
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

        FileShareGovernanceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<FileShareGovernanceChanged>.Success(changed);
    }
}