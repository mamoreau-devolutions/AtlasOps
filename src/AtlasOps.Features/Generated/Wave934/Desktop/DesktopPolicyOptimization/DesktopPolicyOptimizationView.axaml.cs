namespace AtlasOps.Features.Desktop.DesktopPolicyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPolicyOptimizationView : UserControl
{
    public DesktopPolicyOptimizationView()
    {
        this.DataContext = new DesktopPolicyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPolicyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}