namespace AtlasOps.App.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using AtlasOps.App.Infrastructure;
using AtlasOps.App.Services;
using AtlasOps.Composition;
using AtlasOps.Core;
using AtlasOps.Core.Generated;
using AtlasOps.Features;
using AtlasOps.Geography.ViewModels;
using AtlasOps.ReferenceData.ViewModels;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly AtlasOpsBootstrapper bootstrapper;
    private readonly List<IAtlasOpsEntity> entities = [];
    private readonly DelegateCommand deleteEntityCommand;
    private AtlasOpsModelDescriptor? selectedModelDescriptor;
    private AtlasOpsEntitySummary? selectedEntitySummary;
    private IGeneratedEditorViewModel? selectedEditor;
    private string searchText = string.Empty;
    private string statusText = "Starting AtlasOps...";
    private bool isBusy;
    private string selectedRegion = "Workspace";
    private double navigationWidth = 236;
    private double detailsWidth = 292;
    private bool isNavigationCollapsed;
    private AtlasOpsCapabilityDescriptor? selectedCapability;
    private object? selectedCapabilityView;
    private AtlasOpsModuleDescriptor? selectedModule;
    private object? selectedModuleView;

    public MainViewModel(AtlasOpsBootstrapper bootstrapper)
    {
        this.bootstrapper = bootstrapper;
        this.CommandRouter = new AtlasOpsCommandRouter();
        this.Notifications = new AtlasOpsNotificationService();
        this.RefreshCommand = new AsyncDelegateCommand(this.RefreshAsync, this.HandleError);
        this.SaveCommand = new AsyncDelegateCommand(this.SaveAsync, this.HandleError);
        this.AddEntityCommand = new DelegateCommand(this.AddEntity, () => this.SelectedModelDescriptor is not null);
        this.deleteEntityCommand = new DelegateCommand(this.DeleteSelectedEntity, () => this.SelectedEntitySummary is not null);
        this.DeleteEntityCommand = this.deleteEntityCommand;
        this.CommandRouter.Register(AtlasOpsCommandId.Refresh, this.ExecuteRefreshCommandAsync);
        this.CommandRouter.Register(AtlasOpsCommandId.Save, this.ExecuteSaveCommandAsync);
        this.CommandRouter.Register(AtlasOpsCommandId.AddEntity, this.ExecuteAddCommandAsync);
        this.CommandRouter.Register(AtlasOpsCommandId.DeleteEntity, this.ExecuteDeleteCommandAsync);
        this.ModelDescriptors = AtlasOpsGeneratedModelCatalog.Models;
        this.Capabilities = AtlasOpsCapabilityCatalog.All;
        this.Modules = AtlasOpsModuleCatalog.Create();
        this.Geography = GeographyCatalogViewModel.CreateDefault();
        this.IanaReferenceData = ReferenceDataWorkbenchViewModel.CreateIana();
        this.LifecycleReferenceData = ReferenceDataWorkbenchViewModel.CreateLifecycle();
        this.LocalizationReferenceData = ReferenceDataWorkbenchViewModel.CreateLocalization();
        this.CloudReferenceData = ReferenceDataWorkbenchViewModel.CreateCloud();
        this.GeospatialReferenceData = ReferenceDataWorkbenchViewModel.CreateGeospatial();
        this.SelectedModelDescriptor = this.ModelDescriptors.FirstOrDefault();
        this.SelectedCapability = this.Capabilities.FirstOrDefault();
        this.SelectedModule = this.Modules.FirstOrDefault();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AvaloniaList<AtlasOpsProject> Projects { get; } = [];

    public AvaloniaList<AtlasOpsConnection> Connections { get; } = [];

    public AvaloniaList<AtlasOpsTaskItem> Tasks { get; } = [];

    public AvaloniaList<AtlasOpsEditorDocument> Documents { get; } = [];

    public AvaloniaList<AtlasOpsActivity> Activities { get; } = [];

    public AvaloniaList<AtlasOpsEntitySummary> FilteredEntities { get; } = [];

    public IReadOnlyList<AtlasOpsModelDescriptor> ModelDescriptors { get; }

    public IReadOnlyList<AtlasOpsCapabilityDescriptor> Capabilities { get; }

    public IReadOnlyList<AtlasOpsModuleDescriptor> Modules { get; }

    public GeographyCatalogViewModel Geography { get; }

    public ReferenceDataWorkbenchViewModel IanaReferenceData { get; }

    public ReferenceDataWorkbenchViewModel LifecycleReferenceData { get; }

    public ReferenceDataWorkbenchViewModel LocalizationReferenceData { get; }

    public ReferenceDataWorkbenchViewModel CloudReferenceData { get; }

    public ReferenceDataWorkbenchViewModel GeospatialReferenceData { get; }

    public ICommand RefreshCommand { get; }

    public ICommand SaveCommand { get; }

    public ICommand AddEntityCommand { get; }

    public ICommand DeleteEntityCommand { get; }

    public string WorkspaceName => this.bootstrapper.Settings.WorkspaceName;

    public string PersistenceKind => this.bootstrapper.PersistenceKind;

    public AtlasOpsCommandRouter CommandRouter { get; }

    public AtlasOpsNotificationService Notifications { get; }

    public int GeneratedModelCount => this.ModelDescriptors.Count;

    public int GeneratedFieldCount => this.ModelDescriptors.Sum(static descriptor => descriptor.Fields.Count);

    public int CapabilityCount => this.Capabilities.Count;

    public int CapabilityWaveCount => AtlasOpsWaveManifest.CountsByWave.Count;

    public int EntityCount => this.entities.Count;

    public AtlasOpsCapabilityDescriptor? SelectedCapability
    {
        get => this.selectedCapability;
        set
        {
            if (this.selectedCapability == value)
            {
                return;
            }

            this.selectedCapability = value;
            this.SelectedCapabilityView = value?.CreateView();
            this.OnPropertyChanged();
        }
    }

    public object? SelectedCapabilityView
    {
        get => this.selectedCapabilityView;
        private set
        {
            if (this.selectedCapabilityView == value)
            {
                return;
            }

            this.selectedCapabilityView = value;
            this.OnPropertyChanged();
        }
    }

    public AtlasOpsModuleDescriptor? SelectedModule
    {
        get => this.selectedModule;
        set
        {
            if (this.selectedModule == value)
            {
                return;
            }

            this.selectedModule = value;
            this.SelectedModuleView = value?.CreateView();
            this.OnPropertyChanged();
        }
    }

    public object? SelectedModuleView
    {
        get => this.selectedModuleView;
        private set
        {
            if (this.selectedModuleView == value)
            {
                return;
            }

            this.selectedModuleView = value;
            this.OnPropertyChanged();
        }
    }
    public AtlasOpsModelDescriptor? SelectedModelDescriptor
    {
        get => this.selectedModelDescriptor;
        set
        {
            if (this.selectedModelDescriptor == value)
            {
                return;
            }

            this.selectedModelDescriptor = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.SelectedModelTitle));
            this.RefreshEntityFilter();
        }
    }

    public string SelectedModelTitle => this.SelectedModelDescriptor?.DisplayName ?? "Workspace";

    public AtlasOpsEntitySummary? SelectedEntitySummary
    {
        get => this.selectedEntitySummary;
        set
        {
            if (this.selectedEntitySummary == value)
            {
                return;
            }

            this.selectedEntitySummary = value;
            this.SelectedEditor = value is null
                ? null
                : AtlasOpsGeneratedEditorFactory.Create(value.Entity);
            this.OnPropertyChanged();
            this.deleteEntityCommand.NotifyCanExecuteChanged();
        }
    }

    public IGeneratedEditorViewModel? SelectedEditor
    {
        get => this.selectedEditor;
        private set
        {
            if (this.selectedEditor == value)
            {
                return;
            }

            this.selectedEditor = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.HasSelectedEditor));
        }
    }

    public bool HasSelectedEditor => this.SelectedEditor is not null;

    public string SelectedRegion
    {
        get => this.selectedRegion;
        set
        {
            if (this.selectedRegion == value)
            {
                return;
            }

            this.selectedRegion = value;
            this.OnPropertyChanged();
        }
    }

    public double NavigationWidth
    {
        get => this.navigationWidth;
        set
        {
            double normalizedValue = Math.Max(180, value);
            if (Math.Abs(this.navigationWidth - normalizedValue) < 0.1)
            {
                return;
            }

            this.navigationWidth = normalizedValue;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.NavigationColumnWidth));
        }
    }

    public double DetailsWidth
    {
        get => this.detailsWidth;
        set
        {
            double normalizedValue = Math.Max(240, value);
            if (Math.Abs(this.detailsWidth - normalizedValue) < 0.1)
            {
                return;
            }

            this.detailsWidth = normalizedValue;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.DetailsColumnWidth));
        }
    }

    public bool IsNavigationCollapsed
    {
        get => this.isNavigationCollapsed;
        set
        {
            if (this.isNavigationCollapsed == value)
            {
                return;
            }

            this.isNavigationCollapsed = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.NavigationColumnWidth));
        }
    }

    public GridLength NavigationColumnWidth => this.IsNavigationCollapsed
        ? new GridLength(0)
        : new GridLength(this.NavigationWidth);

    public GridLength DetailsColumnWidth => new(this.DetailsWidth);

    public string SearchText
    {
        get => this.searchText;
        set
        {
            if (this.searchText == value)
            {
                return;
            }

            this.searchText = value;
            this.OnPropertyChanged();
            this.RefreshEntityFilter();
        }
    }

    public string StatusText
    {
        get => this.statusText;
        private set
        {
            if (this.statusText == value)
            {
                return;
            }

            this.statusText = value;
            this.OnPropertyChanged();
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
        }
    }

    public async Task InitializeAsync()
    {
        try
        {
            AtlasOpsLayoutState layout = await this.bootstrapper.LoadLayoutAsync();
            this.ApplyLayout(layout);
            await Task.WhenAll(
                this.RefreshAsync(),
                this.Geography.InitializeAsync(),
                this.IanaReferenceData.InitializeAsync(),
                this.LifecycleReferenceData.InitializeAsync(),
                this.LocalizationReferenceData.InitializeAsync(),
                this.CloudReferenceData.InitializeAsync(),
                this.GeospatialReferenceData.InitializeAsync());
        }
        catch (Exception exception)
        {
            this.HandleError(exception);
        }
    }

    public async Task PersistLayoutAsync(CancellationToken cancellationToken = default)
    {
        AtlasOpsLayoutState layout = new()
        {
            SelectedRegion = this.SelectedRegion,
            SelectedModelType = this.SelectedModelDescriptor?.TypeName ?? string.Empty,
            NavigationWidth = this.NavigationWidth,
            DetailsWidth = this.DetailsWidth,
            IsNavigationCollapsed = this.IsNavigationCollapsed,
        };
        await this.bootstrapper.SaveLayoutAsync(layout, cancellationToken);
    }

    private async Task RefreshAsync()
    {
        this.IsBusy = true;
        this.StatusText = $"Loading from {this.PersistenceKind}...";
        try
        {
            AtlasOpsGeneratedWorkspace workspace = await this.bootstrapper.LoadWorkspaceAsync();
            this.entities.Clear();
            this.entities.AddRange(workspace.Entities);
            this.RebuildDashboardCollections();
            this.RefreshEntityFilter();
            this.StatusText = $"Ready · {this.EntityCount} entities · {this.GeneratedModelCount} generated model sets";
            this.Notifications.Publish(this.StatusText, AtlasOpsNotificationSeverity.Success);
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    private async Task SaveAsync()
    {
        this.IsBusy = true;
        this.StatusText = $"Saving to {this.PersistenceKind}...";
        try
        {
            this.AddAuditEvent("Workspace saved", "All generated entity sets were persisted.");
            await this.bootstrapper.SaveWorkspaceAsync(new AtlasOpsGeneratedWorkspace(this.entities.ToList()));
            this.SelectedEditor?.AcceptChanges();
            this.RebuildDashboardCollections();
            this.StatusText = $"Saved {this.EntityCount} entities to {this.PersistenceKind} at {DateTimeOffset.Now:t}";
            this.Notifications.Publish(this.StatusText, AtlasOpsNotificationSeverity.Success);
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    private void AddEntity()
    {
        if (this.SelectedModelDescriptor is null)
        {
            return;
        }

        IAtlasOpsEntity entity = AtlasOpsGeneratedWorkspaceManager.Create(this.SelectedModelDescriptor.TypeName);
        this.entities.Insert(0, entity);
        this.AddAuditEvent(
            $"{this.SelectedModelDescriptor.DisplayName} item created",
            $"Created {entity.Id} through the generated editor.");
        this.RebuildDashboardCollections();
        this.RefreshEntityFilter();
        this.SelectedEntitySummary = this.FilteredEntities.FirstOrDefault(summary => summary.Entity == entity);
        this.OnPropertyChanged(nameof(this.EntityCount));
        this.StatusText = $"Created a new {this.SelectedModelDescriptor.TypeName}; edit its fields and save.";
    }

    private void DeleteSelectedEntity()
    {
        AtlasOpsEntitySummary? selected = this.SelectedEntitySummary;
        if (selected is null)
        {
            return;
        }

        this.entities.Remove(selected.Entity);
        this.AddAuditEvent(
            $"{selected.TypeName} deleted",
            $"Deleted {selected.PrimaryText} ({selected.Entity.Id}).");
        this.SelectedEntitySummary = null;
        this.RebuildDashboardCollections();
        this.RefreshEntityFilter();
        this.OnPropertyChanged(nameof(this.EntityCount));
        this.StatusText = $"Deleted {selected.PrimaryText}; save to persist the change.";
    }

    private void RefreshEntityFilter()
    {
        string? selectedType = this.SelectedModelDescriptor?.TypeName;
        string query = this.SearchText.Trim();
        List<AtlasOpsEntitySummary> summaries = this.entities
            .Select(AtlasOpsGeneratedWorkspaceManager.Summarize)
            .Where(summary => selectedType is null || summary.TypeName == selectedType)
            .Where(summary => string.IsNullOrEmpty(query) ||
                summary.SearchText.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                summary.PrimaryText.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                summary.SecondaryText.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(static summary => summary.Entity.UpdatedAt)
            .ToList();

        ReplaceItems(this.FilteredEntities, summaries);
        if (this.SelectedEntitySummary is not null &&
            !summaries.Any(summary => summary.Entity.Id == this.SelectedEntitySummary.Entity.Id))
        {
            this.SelectedEntitySummary = null;
        }
    }

    private void RebuildDashboardCollections()
    {
        ReplaceItems(this.Projects, this.entities.OfType<AtlasOpsProject>());
        ReplaceItems(this.Connections, this.entities.OfType<AtlasOpsConnection>());
        ReplaceItems(this.Tasks, this.entities.OfType<AtlasOpsTaskItem>());
        ReplaceItems(this.Documents, this.entities.OfType<AtlasOpsEditorDocument>());
        ReplaceItems(
            this.Activities,
            this.entities.OfType<AtlasOpsActivity>().OrderByDescending(static activity => activity.OccurredAt));
        this.OnPropertyChanged(nameof(this.EntityCount));
    }

    private void AddAuditEvent(string title, string description)
    {
        this.entities.Add(new AtlasOpsActivity
        {
            Title = title,
            Kind = "Audit",
            Actor = Environment.UserName,
            Description = description,
            OccurredAt = DateTimeOffset.UtcNow,
            IsUnread = true,
        });
        this.entities.Add(new AtlasOpsAuditEvent
        {
            Action = title,
            Actor = Environment.UserName,
            Target = "AtlasOps workspace",
            Result = "Success",
            Details = description,
            OccurredAt = DateTimeOffset.UtcNow,
        });
    }

    private void HandleError(Exception exception)
    {
        this.IsBusy = false;
        this.StatusText = $"Operation failed: {exception.Message}";
        this.Notifications.Publish(this.StatusText, AtlasOpsNotificationSeverity.Error);
    }

    private async Task ExecuteRefreshCommandAsync(CancellationToken cancellationToken)
    {
        await this.RefreshAsync();
        await this.PersistLayoutAsync(cancellationToken);
    }

    private Task ExecuteAddCommandAsync(CancellationToken cancellationToken)
    {
        this.AddEntity();
        return Task.CompletedTask;
    }

    private Task ExecuteDeleteCommandAsync(CancellationToken cancellationToken)
    {
        this.DeleteSelectedEntity();
        return Task.CompletedTask;
    }

    private async Task ExecuteSaveCommandAsync(CancellationToken cancellationToken)
    {
        await this.SaveAsync();
        await this.PersistLayoutAsync(cancellationToken);
    }

    private void ApplyLayout(AtlasOpsLayoutState layout)
    {
        this.SelectedRegion = layout.SelectedRegion;
        this.NavigationWidth = layout.NavigationWidth;
        this.DetailsWidth = layout.DetailsWidth;
        this.IsNavigationCollapsed = layout.IsNavigationCollapsed;
        this.SelectedModelDescriptor = this.ModelDescriptors
            .FirstOrDefault(descriptor => descriptor.TypeName == layout.SelectedModelType)
            ?? this.SelectedModelDescriptor;
    }

    private static void ReplaceItems<T>(AvaloniaList<T> target, IEnumerable<T> source)
    {
        target.Clear();
        target.AddRange(source);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}