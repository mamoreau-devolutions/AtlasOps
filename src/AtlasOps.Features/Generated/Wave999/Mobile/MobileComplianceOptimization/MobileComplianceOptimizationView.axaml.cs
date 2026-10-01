namespace AtlasOps.Features.Mobile.MobileComplianceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileComplianceOptimizationView : UserControl
{
    public MobileComplianceOptimizationView()
    {
        this.DataContext = new MobileComplianceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileComplianceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}