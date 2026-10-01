namespace AtlasOps.Features.Mobile.MobileApplicationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileApplicationOptimizationView : UserControl
{
    public MobileApplicationOptimizationView()
    {
        this.DataContext = new MobileApplicationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileApplicationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}