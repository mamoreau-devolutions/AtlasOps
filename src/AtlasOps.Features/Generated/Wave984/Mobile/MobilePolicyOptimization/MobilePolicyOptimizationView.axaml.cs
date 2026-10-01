namespace AtlasOps.Features.Mobile.MobilePolicyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobilePolicyOptimizationView : UserControl
{
    public MobilePolicyOptimizationView()
    {
        this.DataContext = new MobilePolicyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobilePolicyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}