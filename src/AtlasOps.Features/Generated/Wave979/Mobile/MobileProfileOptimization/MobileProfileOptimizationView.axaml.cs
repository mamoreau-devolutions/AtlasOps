namespace AtlasOps.Features.Mobile.MobileProfileOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileProfileOptimizationView : UserControl
{
    public MobileProfileOptimizationView()
    {
        this.DataContext = new MobileProfileOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileProfileOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}