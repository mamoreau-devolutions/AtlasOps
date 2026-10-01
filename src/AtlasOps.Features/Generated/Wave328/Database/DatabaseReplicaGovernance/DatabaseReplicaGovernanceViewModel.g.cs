namespace AtlasOps.Features.Database.DatabaseReplicaGovernance;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class DatabaseReplicaGovernanceViewModel : INotifyPropertyChanged
{
    private readonly DatabaseReplicaGovernancePolicy policy = new();
    private string state = "Draft";
    private string status = "Ready for Database Replica Governance operations.";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => "Database Replica Governance";

    public string Area => "Database";

    public int Wave => 328;

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