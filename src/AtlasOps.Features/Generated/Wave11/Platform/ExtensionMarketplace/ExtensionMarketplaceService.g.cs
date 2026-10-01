namespace AtlasOps.Features.Platform.ExtensionMarketplace;

using AtlasOps.Features;

public sealed class ExtensionMarketplaceService(
    IAtlasOpsCapabilityRepository<ExtensionMarketplaceItem> repository,
    TimeProvider timeProvider)
{
    private readonly ExtensionMarketplaceValidator validator = new();
    private readonly ExtensionMarketplacePolicy policy = new();

    public async Task<AtlasOpsOperationResult<ExtensionMarketplaceChanged>> ExecuteAsync(
        UpdateExtensionMarketplaceCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ExtensionMarketplaceChanged>.Invalid(issues);
        }

        ExtensionMarketplaceItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ExtensionMarketplaceItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ExtensionMarketplaceChanged>.Invalid(
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

        ExtensionMarketplaceChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ExtensionMarketplaceChanged>.Success(changed);
    }
}