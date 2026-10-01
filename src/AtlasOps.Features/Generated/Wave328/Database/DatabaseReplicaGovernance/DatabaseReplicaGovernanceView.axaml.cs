namespace AtlasOps.Features.Database.DatabaseReplicaGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseReplicaGovernanceView : UserControl
{
    public DatabaseReplicaGovernanceView()
    {
        this.DataContext = new DatabaseReplicaGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseReplicaGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}