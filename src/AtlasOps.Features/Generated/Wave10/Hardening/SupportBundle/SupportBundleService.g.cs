namespace AtlasOps.Features.Hardening.SupportBundle;

using AtlasOps.Features;

public sealed class SupportBundleService(
    IAtlasOpsCapabilityRepository<SupportBundleItem> repository,
    TimeProvider timeProvider)
{
    private readonly SupportBundleValidator validator = new();
    private readonly SupportBundlePolicy policy = new();

    public async Task<AtlasOpsOperationResult<SupportBundleChanged>> ExecuteAsync(
        UpdateSupportBundleCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SupportBundleChanged>.Invalid(issues);
        }

        SupportBundleItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SupportBundleItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SupportBundleChanged>.Invalid(
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

        SupportBundleChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SupportBundleChanged>.Success(changed);
    }
}