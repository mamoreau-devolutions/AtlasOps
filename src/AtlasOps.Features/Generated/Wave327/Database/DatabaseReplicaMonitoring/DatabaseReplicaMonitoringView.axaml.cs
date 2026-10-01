namespace AtlasOps.Features.Database.DatabaseReplicaMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseReplicaMonitoringView : UserControl
{
    public DatabaseReplicaMonitoringView()
    {
        this.DataContext = new DatabaseReplicaMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseReplicaMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}