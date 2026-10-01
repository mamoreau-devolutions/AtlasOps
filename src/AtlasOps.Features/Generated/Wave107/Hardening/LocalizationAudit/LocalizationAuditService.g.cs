namespace AtlasOps.Features.Hardening.LocalizationAudit;

using AtlasOps.Features;

public sealed class LocalizationAuditService(
    IAtlasOpsCapabilityRepository<LocalizationAuditItem> repository,
    TimeProvider timeProvider)
{
    private readonly LocalizationAuditValidator validator = new();
    private readonly LocalizationAuditPolicy policy = new();

    public async Task<AtlasOpsOperationResult<LocalizationAuditChanged>> ExecuteAsync(
        UpdateLocalizationAuditCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<LocalizationAuditChanged>.Invalid(issues);
        }

        LocalizationAuditItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new LocalizationAuditItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<LocalizationAuditChanged>.Invalid(
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

        LocalizationAuditChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<LocalizationAuditChanged>.Success(changed);
    }
}