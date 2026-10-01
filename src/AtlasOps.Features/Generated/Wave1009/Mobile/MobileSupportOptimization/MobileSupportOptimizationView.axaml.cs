namespace AtlasOps.Features.Mobile.MobileSupportOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileSupportOptimizationView : UserControl
{
    public MobileSupportOptimizationView()
    {
        this.DataContext = new MobileSupportOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileSupportOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}