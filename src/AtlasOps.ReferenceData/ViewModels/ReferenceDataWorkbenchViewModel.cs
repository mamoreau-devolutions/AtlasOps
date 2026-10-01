namespace AtlasOps.ReferenceData.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using AtlasOps.ReferenceData.Cloud;
using AtlasOps.ReferenceData.Geospatial;
using AtlasOps.ReferenceData.Iana;
using AtlasOps.ReferenceData.Infrastructure;
using AtlasOps.ReferenceData.Lifecycle;
using AtlasOps.ReferenceData.Localization;

using Avalonia.Collections;

public sealed record ReferenceMetricViewModel(string Name, string Value);

public sealed class ReferenceDataWorkbenchViewModel : INotifyPropertyChanged
{
    private readonly Func<CancellationToken, Task<ReferenceWorkbenchSnapshot>> loader;
    private ReferenceSearchIndex? searchIndex;
    private ReferenceDataRow? selectedRow;
    private ReferenceValidationIssue? selectedIssue;
    private string searchText = string.Empty;
    private string title;
    private string description;
    private string statusText = "Reference pack is not loaded.";
    private string releaseText = string.Empty;
    private bool isBusy;

    private ReferenceDataWorkbenchViewModel(
        string title,
        string description,
        Func<CancellationToken, Task<ReferenceWorkbenchSnapshot>> loader)
    {
        this.title = title;
        this.description = description;
        this.loader = loader;
        this.RefreshCommand = new AsyncCommand(this.InitializeAsync, this.HandleError);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public AvaloniaList<ReferenceDataRow> Rows { get; } = [];

    public AvaloniaList<ReferenceValidationIssue> Issues { get; } = [];

    public AvaloniaList<ReferenceMetricViewModel> Metrics { get; } = [];

    public ICommand RefreshCommand { get; }

    public string Title
    {
        get => this.title;
        private set => this.SetField(ref this.title, value);
    }

    public string Description
    {
        get => this.description;
        private set => this.SetField(ref this.description, value);
    }

    public string SearchText
    {
        get => this.searchText;
        set
        {
            if (this.SetField(ref this.searchText, value))
            {
                this.ApplySearch();
            }
        }
    }

    public string StatusText
    {
        get => this.statusText;
        private set => this.SetField(ref this.statusText, value);
    }

    public string ReleaseText
    {
        get => this.releaseText;
        private set => this.SetField(ref this.releaseText, value);
    }

    public bool IsBusy
    {
        get => this.isBusy;
        private set => this.SetField(ref this.isBusy, value);
    }

    public ReferenceDataRow? SelectedRow
    {
        get => this.selectedRow;
        set => this.SetField(ref this.selectedRow, value);
    }

    public ReferenceValidationIssue? SelectedIssue
    {
        get => this.selectedIssue;
        set => this.SetField(ref this.selectedIssue, value);
    }

    public static ReferenceDataWorkbenchViewModel CreateIana()
    {
        return new ReferenceDataWorkbenchViewModel(
            "IANA network registries",
            "Authoritative protocol assignments and TLS policy.",
            IanaWorkbenchBuilder.BuildAsync);
    }

    public static ReferenceDataWorkbenchViewModel CreateLifecycle()
    {
        return new ReferenceDataWorkbenchViewModel(
            "Software lifecycle intelligence",
            "Release and end-of-life risk across hundreds of products.",
            LifecycleWorkbenchBuilder.BuildAsync);
    }

    public static ReferenceDataWorkbenchViewModel CreateLocalization()
    {
        return new ReferenceDataWorkbenchViewModel(
            "Locale and time-zone intelligence",
            "Unicode CLDR localization coverage and IANA time-zone mappings.",
            LocalizationWorkbenchBuilder.BuildAsync);
    }

    public static ReferenceDataWorkbenchViewModel CreateCloud()
    {
        return new ReferenceDataWorkbenchViewModel(
            "Multi-cloud instance economics",
            "Normalized regional AWS, Azure, and GCP compute pricing.",
            CloudWorkbenchBuilder.BuildAsync);
    }

    public static ReferenceDataWorkbenchViewModel CreateGeospatial()
    {
        return new ReferenceDataWorkbenchViewModel(
            "Natural Earth geospatial operations",
            "Global administrative and infrastructure feature analysis.",
            GeospatialWorkbenchBuilder.BuildAsync);
    }

    public async Task InitializeAsync()
    {
        this.IsBusy = true;
        this.StatusText = "Loading and validating reference data...";
        try
        {
            ReferenceWorkbenchSnapshot snapshot = await this.loader(CancellationToken.None);
            this.Title = snapshot.Title;
            this.Description = snapshot.Description;
            this.searchIndex = new ReferenceSearchIndex(snapshot.Rows);
            this.Issues.Clear();
            this.Issues.AddRange(snapshot.Issues.Take(500));
            this.Metrics.Clear();
            this.Metrics.AddRange(snapshot.Metrics.Select(static item =>
                new ReferenceMetricViewModel(item.Key, item.Value)));
            this.ReleaseText =
                $"{snapshot.Manifest.Version} · {snapshot.Manifest.License} · {snapshot.Manifest.FileCount:N0} files · {snapshot.Manifest.RetrievedAt:yyyy-MM-dd}";
            this.ApplySearch();
            this.StatusText =
                $"Loaded {snapshot.Rows.Count:N0} searchable records with {snapshot.Issues.Count:N0} validation findings.";
        }
        finally
        {
            this.IsBusy = false;
        }
    }

    private void ApplySearch()
    {
        if (this.searchIndex is null)
        {
            return;
        }

        this.Rows.Clear();
        this.Rows.AddRange(this.searchIndex.Search(this.SearchText, 500));
        this.SelectedRow = this.Rows.FirstOrDefault();
    }

    private void HandleError(Exception exception)
    {
        this.IsBusy = false;
        this.StatusText = $"Reference-data operation failed: {exception.Message}";
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}