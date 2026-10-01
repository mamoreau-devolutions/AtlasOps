namespace AtlasOps.Features.Mobile.MobileDeviceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileDeviceOptimizationView : UserControl
{
    public MobileDeviceOptimizationView()
    {
        this.DataContext = new MobileDeviceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileDeviceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}