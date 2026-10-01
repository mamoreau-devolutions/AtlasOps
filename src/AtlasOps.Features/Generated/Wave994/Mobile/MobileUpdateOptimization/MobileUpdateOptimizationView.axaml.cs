namespace AtlasOps.Features.Mobile.MobileUpdateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileUpdateOptimizationView : UserControl
{
    public MobileUpdateOptimizationView()
    {
        this.DataContext = new MobileUpdateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileUpdateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}