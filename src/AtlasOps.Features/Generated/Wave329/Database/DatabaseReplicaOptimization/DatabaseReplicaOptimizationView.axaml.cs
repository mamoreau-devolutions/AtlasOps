namespace AtlasOps.Features.Database.DatabaseReplicaOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseReplicaOptimizationView : UserControl
{
    public DatabaseReplicaOptimizationView()
    {
        this.DataContext = new DatabaseReplicaOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseReplicaOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}