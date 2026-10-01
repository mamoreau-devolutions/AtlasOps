namespace AtlasOps.Operations.Avalonia;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using global::Avalonia.Collections;

public class OperationsWorkbenchViewModel : INotifyPropertyChanged
{
    private readonly List<OperationRowViewModel> sourceRows;
    private string searchText = string.Empty;
    private OperationRowViewModel? selectedRow;

    public OperationsWorkbenchViewModel(
        string title,
        string summary,
        IEnumerable<OperationRowViewModel> rows)
    {
        this.Title = title;
        this.Summary = summary;
        this.sourceRows = rows.ToList();
        this.RefreshCommand = new WorkbenchCommand(this.RefreshFilter);
        this.RetryCommand = new WorkbenchCommand(
            () => this.UpdateSelectedStatus("Retry requested", "Operation was queued for another attempt."),
            () => this.SelectedRow is not null);
        this.CancelCommand = new WorkbenchCommand(
            () => this.UpdateSelectedStatus("Cancellation requested", "Cancellation will be applied at the next safe checkpoint."),
            () => this.SelectedRow is not null);
        this.RefreshFilter();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title { get; }

    public string Summary { get; }

    public AvaloniaList<OperationRowViewModel> Rows { get; } = [];

    public ICommand RefreshCommand { get; }

    public ICommand RetryCommand { get; }

    public ICommand CancelCommand { get; }

    public string SearchText
    {
        get => this.searchText;
        set
        {
            string next = value ?? string.Empty;
            if (string.Equals(this.searchText, next, StringComparison.Ordinal))
            {
                return;
            }

            this.searchText = next;
            this.OnPropertyChanged();
            this.RefreshFilter();
        }
    }

    public OperationRowViewModel? SelectedRow
    {
        get => this.selectedRow;
        set
        {
            if (Equals(this.selectedRow, value))
            {
                return;
            }

            this.selectedRow = value;
            this.OnPropertyChanged();
            ((WorkbenchCommand)this.RetryCommand).NotifyCanExecuteChanged();
            ((WorkbenchCommand)this.CancelCommand).NotifyCanExecuteChanged();
        }
    }

    public int TotalCount => this.sourceRows.Count;

    public int VisibleCount => this.Rows.Count;

    private void RefreshFilter()
    {
        string filter = this.searchText.Trim();
        IEnumerable<OperationRowViewModel> rows = this.sourceRows;
        if (filter.Length > 0)
        {
            rows = rows.Where(row =>
                row.Title.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                row.Category.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                row.Status.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                row.Detail.Contains(filter, StringComparison.OrdinalIgnoreCase));
        }

        this.Rows.Clear();
        this.Rows.AddRange(rows
            .OrderByDescending(static row => row.UpdatedAt)
            .ThenBy(static row => row.Title, StringComparer.OrdinalIgnoreCase));
        this.OnPropertyChanged(nameof(this.VisibleCount));
    }

    private void UpdateSelectedStatus(string status, string detail)
    {
        if (this.SelectedRow is null)
        {
            return;
        }

        int index = this.sourceRows.FindIndex(
            row => string.Equals(row.Id, this.SelectedRow.Id, StringComparison.Ordinal));
        if (index < 0)
        {
            return;
        }

        OperationRowViewModel updated = this.SelectedRow with
        {
            Status = status,
            Detail = detail,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        this.sourceRows[index] = updated;
        this.RefreshFilter();
        this.SelectedRow = this.Rows.FirstOrDefault(
            row => string.Equals(row.Id, updated.Id, StringComparison.Ordinal));
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class WorkbenchCommand(
        Action execute,
        Func<bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            execute();
        }

        public void NotifyCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
