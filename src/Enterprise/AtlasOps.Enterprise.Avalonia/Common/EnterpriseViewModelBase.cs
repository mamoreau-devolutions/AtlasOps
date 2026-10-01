namespace AtlasOps.Enterprise.Avalonia.Common;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public abstract class EnterpriseViewModelBase : INotifyPropertyChanged
{
    private string searchText = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public abstract string Title { get; }

    public abstract string Summary { get; }

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
            this.OnSearchChanged();
        }
    }

    protected virtual void OnSearchChanged()
    {
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed record EnterpriseMetric(string Label, string Value, string Detail);

public sealed record EnterpriseDetailRow(string Title, string Subtitle, string Status);
