namespace AtlasOps.Connectors.Avalonia;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using AtlasOps.Connectors.Contracts;
using AtlasOps.Connectors.Runtime;

using global::Avalonia.Collections;

public sealed class ConnectorWorkbenchViewModel : INotifyPropertyChanged
{
    private readonly ConnectorRegistry registry;
    private readonly ConnectorHealthMonitor healthMonitor;
    private readonly ConnectorRuntime runtime;
    private readonly InMemoryConnectorAuditSink auditSink;
    private string searchText = string.Empty;
    private ConnectorDefinition? selectedConnector;
    private bool isBusy;
    private string statusMessage = "Ready";

    public ConnectorWorkbenchViewModel()
        : this(ConnectorScenarioCatalog.CreateRegistry())
    {
    }

    public ConnectorWorkbenchViewModel(ConnectorRegistry registry)
    {
        this.registry = registry;
        this.auditSink = new InMemoryConnectorAuditSink(500);
        this.healthMonitor = new ConnectorHealthMonitor(registry);
        this.runtime = new ConnectorRuntime(registry, new ConnectorRateLimiter(), this.auditSink);
        this.RefreshCommand = new AsyncConnectorCommand(this.RefreshAsync, () => !this.IsBusy);
        this.ExecuteSelectedCommand = new AsyncConnectorCommand(
            this.ExecuteSelectedAsync,
            () => !this.IsBusy && this.SelectedConnector is not null);
        this.RefreshDefinitions();
        this.SelectedConnector = this.Connectors.FirstOrDefault();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "Connector operations";

    public string Summary => "Inspect connector health, execute reference synchronization, and review bounded audit history.";

    public AvaloniaList<ConnectorDefinition> Connectors { get; } = [];

    public AvaloniaList<ConnectorHealthSnapshot> Health { get; } = [];

    public AvaloniaList<ConnectorAuditRecord> Executions { get; } = [];

    public ICommand RefreshCommand { get; }

    public ICommand ExecuteSelectedCommand { get; }

    public string SearchText
    {
        get => this.searchText;
        set
        {
            string normalized = value ?? string.Empty;
            if (string.Equals(this.searchText, normalized, StringComparison.Ordinal))
            {
                return;
            }

            this.searchText = normalized;
            this.OnPropertyChanged();
            this.RefreshDefinitions();
        }
    }

    public ConnectorDefinition? SelectedConnector
    {
        get => this.selectedConnector;
        set
        {
            if (this.selectedConnector == value)
            {
                return;
            }

            this.selectedConnector = value;
            this.OnPropertyChanged();
            ((AsyncConnectorCommand)this.ExecuteSelectedCommand).NotifyCanExecuteChanged();
        }
    }

    public bool IsBusy
    {
        get => this.isBusy;
        private set
        {
            if (this.isBusy == value)
            {
                return;
            }

            this.isBusy = value;
            this.OnPropertyChanged();
            ((AsyncConnectorCommand)this.RefreshCommand).NotifyCanExecuteChanged();
            ((AsyncConnectorCommand)this.ExecuteSelectedCommand).NotifyCanExecuteChanged();
        }
    }

    public string StatusMessage
    {
        get => this.statusMessage;
        private set
        {
            if (string.Equals(this.statusMessage, value, StringComparison.Ordinal))
            {
                return;
            }

            this.statusMessage = value;
            this.OnPropertyChanged();
        }
    }

    public async Task RefreshAsync()
    {
        this.IsBusy = true;
        try
        {
            IReadOnlyList<ConnectorHealthSnapshot> health =
                await this.healthMonitor.RefreshAllAsync(CancellationToken.None).ConfigureAwait(true);
            this.Health.Clear();
            this.Health.AddRange(health);
            this.RefreshExecutions();
            this.StatusMessage = $"Refreshed {health.Count} connector health checks.";
        }
        catch (Exception exception)
        {
            this.StatusMessage = $"Refresh failed: {exception.Message}";
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    public async Task ExecuteSelectedAsync()
    {
        ConnectorDefinition? selected = this.SelectedConnector;
        if (selected is null)
        {
            return;
        }

        this.IsBusy = true;
        try
        {
            ConnectorExecutionRequest request = new(
                Guid.NewGuid(),
                selected.Id,
                "synchronize",
                "workbench",
                null,
                DateTimeOffset.UtcNow,
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                Guid.NewGuid().ToString("N"));
            ConnectorExecutionOutcome outcome =
                await this.runtime.ExecuteAsync(request, CancellationToken.None).ConfigureAwait(true);
            this.RefreshExecutions();
            this.StatusMessage = $"{selected.DisplayName}: {outcome.Message}";
        }
        catch (Exception exception)
        {
            this.StatusMessage = $"Execution failed: {exception.Message}";
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    private void RefreshDefinitions()
    {
        ConnectorDefinition? previousSelection = this.SelectedConnector;
        this.Connectors.Clear();
        this.Connectors.AddRange(this.registry.Search(this.SearchText));
        if (previousSelection is not null && this.Connectors.Contains(previousSelection))
        {
            this.SelectedConnector = previousSelection;
        }
        else
        {
            this.SelectedConnector = this.Connectors.FirstOrDefault();
        }
    }

    private void RefreshExecutions()
    {
        this.Executions.Clear();
        this.Executions.AddRange(
            this.auditSink.Records
                .OrderByDescending(static item => item.OccurredAt)
                .Take(100));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class AsyncConnectorCommand(Func<Task> execute, Func<bool> canExecute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute();
        }

        public async void Execute(object? parameter)
        {
            await execute().ConfigureAwait(true);
        }

        public void NotifyCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
