namespace AtlasOps.App.Infrastructure;

public enum AtlasOpsCommandId
{
    Refresh,
    Save,
    AddEntity,
    DeleteEntity,
    OpenSettings,
}

public sealed record AtlasOpsDialogRequest(
    string Title,
    string Message,
    AtlasOpsNotificationSeverity Severity = AtlasOpsNotificationSeverity.Information);

public interface IAtlasOpsDialogService
{
    Task<bool> ShowAsync(AtlasOpsDialogRequest request, CancellationToken cancellationToken = default);
}

public sealed class AtlasOpsDialogService : IAtlasOpsDialogService
{
    private readonly Func<AtlasOpsDialogRequest, CancellationToken, Task<bool>> showDialog;

    public AtlasOpsDialogService(Func<AtlasOpsDialogRequest, CancellationToken, Task<bool>> showDialog)
    {
        this.showDialog = showDialog ?? throw new ArgumentNullException(nameof(showDialog));
    }

    public Task<bool> ShowAsync(AtlasOpsDialogRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return this.showDialog(request, cancellationToken);
    }
}

public sealed class AtlasOpsCommandRouter
{
    private readonly Dictionary<AtlasOpsCommandId, Func<CancellationToken, Task>> handlers = [];

    public event EventHandler<AtlasOpsCommandId>? CommandExecuted;

    public void Register(AtlasOpsCommandId commandId, Func<CancellationToken, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        this.handlers[commandId] = handler;
    }

    public bool CanExecute(AtlasOpsCommandId commandId)
    {
        return this.handlers.ContainsKey(commandId);
    }

    public async Task ExecuteAsync(AtlasOpsCommandId commandId, CancellationToken cancellationToken = default)
    {
        if (!this.handlers.TryGetValue(commandId, out Func<CancellationToken, Task>? handler))
        {
            throw new InvalidOperationException($"No handler is registered for {commandId}.");
        }

        await handler(cancellationToken);
        this.CommandExecuted?.Invoke(this, commandId);
    }
}