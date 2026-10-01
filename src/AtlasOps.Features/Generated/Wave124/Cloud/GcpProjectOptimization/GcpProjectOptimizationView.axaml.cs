namespace AtlasOps.Features.Cloud.GcpProjectOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class GcpProjectOptimizationView : UserControl
{
    public GcpProjectOptimizationView()
    {
        this.DataContext = new GcpProjectOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is GcpProjectOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}