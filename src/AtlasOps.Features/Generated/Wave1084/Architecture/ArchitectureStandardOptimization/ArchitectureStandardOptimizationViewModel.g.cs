namespace AtlasOps.Features.Architecture.ArchitectureStandardOptimization;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class ArchitectureStandardOptimizationViewModel : INotifyPropertyChanged
{
    private readonly ArchitectureStandardOptimizationPolicy policy = new();
    private string state = "Draft";
    private string status = "Ready for Architecture Standard Optimization operations.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "Architecture Standard Optimization";

    public string Area => "Architecture";

    public int Wave => 1084;

    public string State
    {
        get => this.state;
        private set
        {
            if (string.Equals(this.state, value, StringComparison.Ordinal))
            {
                return;
            }

            this.state = value;
            this.OnPropertyChanged();
            this.OnPropertyChanged(nameof(this.AvailableActions));
        }
    }

    public string Status
    {
        get => this.status;
        private set
        {
            this.status = value;
            this.OnPropertyChanged();
        }
    }

    public IReadOnlyList<string> AvailableActions => this.policy.GetAvailableTransitions(this.State);

    public void Advance()
    {
        string? next = this.AvailableActions.FirstOrDefault();
        if (next is null)
        {
            this.Status = $"{this.Title} is in terminal state {this.State}.";
            return;
        }

        this.State = next;
        this.Status = $"{this.Title} moved to {this.State}.";
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new(propertyName));
    }
}