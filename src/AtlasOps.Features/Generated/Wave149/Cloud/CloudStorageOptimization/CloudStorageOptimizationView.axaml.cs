namespace AtlasOps.Features.Cloud.CloudStorageOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudStorageOptimizationView : UserControl
{
    public CloudStorageOptimizationView()
    {
        this.DataContext = new CloudStorageOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudStorageOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}