namespace AtlasOps.Features.Cloud.CloudDatabaseOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudDatabaseOptimizationView : UserControl
{
    public CloudDatabaseOptimizationView()
    {
        this.DataContext = new CloudDatabaseOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudDatabaseOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}